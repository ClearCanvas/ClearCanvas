#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.TestApp
{
    public class ImageServerDbGenerator
    {
      
        private DateTime _startDate;
        private int _totalStudies;
        private int _completedStudies;
        private int _studiesPerDay;
        private int _percentWeekend;
        private BackgroundTask _backroundTask ;
        private ServerPartition _partition;
        private List<SopGenerator> _generator = new List<SopGenerator>();
        private Random _rand = new Random();
        private string _aeTitle;
        private string _host;
        private int _port; 

        public ImageServerDbGenerator(ServerPartition partition, DateTime startDate, int totalStudies, int studiesPerDay, int percentWeekend)
        {
            _startDate = startDate;
            _totalStudies = totalStudies;
            _studiesPerDay = studiesPerDay;
            _percentWeekend = percentWeekend;
            _partition = partition;
            _backroundTask = new BackgroundTask(Run, true);
        }

        public ImageServerDbGenerator(string aeTitle, string host, int port, DateTime startDate, int totalStudies, int studiesPerDay, int percentWeekend)
        {
            _startDate = startDate;
            _totalStudies = totalStudies;
            _studiesPerDay = studiesPerDay;
            _percentWeekend = percentWeekend;
            _partition = null;
            _aeTitle = aeTitle;
            _host = host;
            _port = port;
            _backroundTask = new BackgroundTask(Run, true);
        }

        public void RegisterProgressUpated(EventHandler<BackgroundTaskProgressEventArgs> e)
        {
            _backroundTask.ProgressUpdated += e;
        }

        public void AddSopGenerator(SopGenerator generator)
        {
            _generator.Add(generator);    
        }

        public void Start()
        {
            if (!_backroundTask.IsRunning)
            {
                _completedStudies = 0;
                _backroundTask.Run();
            }
        }

        public void Cancel()
        {
            if (_backroundTask !=null && _backroundTask.IsRunning)
                _backroundTask.RequestCancel();
        }

        private void Run(IBackgroundTaskContext context)
        {
            SopGenerator[] generatorArray = _generator.ToArray();
            int completedDayStudies = 0;
            DateTime currentDay = _startDate;

            int currentStudiesPerDay = currentDay.DayOfWeek == DayOfWeek.Saturday ||
                                       currentDay.DayOfWeek == DayOfWeek.Sunday
                                           ? _studiesPerDay*_percentWeekend/100
                                           : _studiesPerDay;
            _completedStudies = 0;
            while (_completedStudies < _totalStudies-1)
            {

                InsertStudy(generatorArray[_rand.Next(1,generatorArray.Length)],currentDay);
                completedDayStudies++;
                _completedStudies++;
                if (completedDayStudies > currentStudiesPerDay)
                {
                    currentDay = currentDay.AddDays(1);
                    currentStudiesPerDay = currentDay.DayOfWeek == DayOfWeek.Saturday ||
                                       currentDay.DayOfWeek == DayOfWeek.Sunday
                                           ? _studiesPerDay * _percentWeekend / 100
                                           : _studiesPerDay;
                    completedDayStudies = 0;
                }

                if ((_completedStudies % 10) == 0)
                    context.ReportProgress(new BackgroundTaskProgress(_completedStudies, _totalStudies, "Studies Created"));
                if (context.CancelRequested)
                    return;
            }
        }

        private void InsertStudy(SopGenerator generator, DateTime currentDay)
        {
            try
            {
                DicomFile file = generator.NewStudy(currentDay);

                if (_partition == null)
                {
                    var scu = new StorageScu("TESTTOOL", _aeTitle, _host, _port);

                    scu.AddStorageInstance(new StorageInstance(file));
                    int series = _rand.Next(1, generator.MaxSeries);
                    for (int i = 1; i < series; i++)
                    {
                        file = generator.NewSeries();
                        scu.AddStorageInstance(new StorageInstance(file));
                    }
                    scu.Send();
                }
                else
                {


                    InsertInstance(file);
                    int series = _rand.Next(1, generator.MaxSeries);
                    for (int i = 1; i < series; i++)
                    {
                        file = generator.NewSeries();
                        InsertInstance(file);
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexecpted exception inserting instance into the database.");
            }
        }

     

        private void InsertInstance(DicomFile file)
        {
            StudyStorageLocation location;
            string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
            string studyDate = file.DataSet[DicomTags.StudyDate].ToString();
            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                bool created;
                location = FilesystemMonitor.Instance.GetOrCreateWritableStudyStorageLocation(studyInstanceUid,
                                                                                              studyDate,
                                                                                              TransferSyntax.ExplicitVrLittleEndian, context,
                                                                                              _partition,
                                                                                              out created);
                context.Commit();
            }

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM file"))
            {
                try
                {
                    // Insert into the database, but only if its not a duplicate so the counts don't get off
                    InsertInstanceCommand insertInstanceCommand = new InsertInstanceCommand(file, location);
                    processor.AddCommand(insertInstanceCommand);

                  
                    // Do the actual processing
                    if (!processor.Execute())
                    {
                        Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
                        Platform.Log(LogLevel.Error, "File that failed processing: {0}", file.Filename);
                        throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid, processor.FailureException);
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                                 processor.Description);
                    processor.Rollback();
                    throw new ApplicationException("Unexpected exception when processing file.", e);
                }
            }
        }
    }
}
