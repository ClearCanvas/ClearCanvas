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

#if UNIT_TESTS

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
    public class StudyBuilderFactory
    {
        #region Mammo

        public static StudyBuilder CreateDigitalMammoBuilder
        (
            string patientId, 
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string seriesDescription,
            int startSeriesNumber,
            bool createSingleImageDisplaySets
        )
        {
            return new StudyBuilder
            {
                DisplaySetFactory = new BasicDisplaySetFactory{CreateSingleImageDisplaySets = createSingleImageDisplaySets},
                PatientId = patientId,
                PatientsName = patientsName,
                StudyDescription = studyDescription,
                StudyInstanceUid = studyInstanceUid,
                Series = new List<SeriesBuilder>
                                {
                                    CreateDigitalMammoSeriesBuilder(seriesDescription, startSeriesNumber, true),
                                    CreateDigitalMammoSeriesBuilder(seriesDescription, startSeriesNumber + 1, false)
                                }
            };
        }

        public static SeriesBuilder CreateDigitalMammoSeriesBuilder(string seriesDescription, int seriesNumber, bool forProcessing)
        {
            var series = new SeriesBuilder
            {
                Modality = "MG",
                SeriesDescription = seriesDescription,
                SeriesNumber = seriesNumber
            };

            var sopClass = forProcessing
                               ? SopClass.DigitalMammographyXRayImageStorageForProcessingUid
                               : SopClass.DigitalMammographyXRayImageStorageForPresentationUid;

            var presentationIntentType = forProcessing ? "FOR PROCESSING" : "FOR PRESENTATION";
            series.Images = new List<ImageBuilder>
                                    {
                                        new ImageBuilder {ImageLaterality = "L", ViewPosition = "MLO", InstanceNumber = 1, SopClassUid = sopClass, PresentationIntentType = presentationIntentType },
                                        new ImageBuilder {ImageLaterality = "L", ViewPosition = "CC", InstanceNumber = 2, SopClassUid = sopClass , PresentationIntentType = presentationIntentType },
                                        new ImageBuilder {ImageLaterality = "R", ViewPosition = "MLO", InstanceNumber = 3, SopClassUid = sopClass , PresentationIntentType = presentationIntentType },
                                        new ImageBuilder {ImageLaterality = "R", ViewPosition = "CC", InstanceNumber = 4, SopClassUid = sopClass , PresentationIntentType = presentationIntentType }
                                    };
            return series;
        }

        #endregion

        #region Digital X-Ray

        public static StudyBuilder CreateDigitalXRayBuilder
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string seriesDescription,
            int startSeriesNumber,
            bool oneImagePerSeries,
            bool createSingleImageDisplaySets
        )
        {
            return CreateDigitalXRayBuilder(patientId, patientsName, studyDescription,
                                            studyInstanceUid, seriesDescription, startSeriesNumber, 
                                            !oneImagePerSeries, oneImagePerSeries, createSingleImageDisplaySets);
        }

        public static StudyBuilder CreateDigitalXRayBuilder
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string seriesDescription,
            int startSeriesNumber,
            bool oneSeries,
            bool oneImagePerSeries,
            bool createSingleImageDisplaySets
        )
        {
            if (!oneSeries && !oneImagePerSeries)
                throw new ArgumentException("Multiple Series currently restricted to one image each");

            var studyBuilder = new StudyBuilder
            {
                DisplaySetFactory = new BasicDisplaySetFactory { CreateSingleImageDisplaySets = createSingleImageDisplaySets },
                PatientId = patientId,
                PatientsName = patientsName,
                StudyDescription = studyDescription,
                StudyInstanceUid = studyInstanceUid
            };

            var image1 = new ImageBuilder
                             {
                                 ImageLaterality = "U",
                                 ViewPosition = "AP",
                                 InstanceNumber = 1,
                                 SopClassUid = SopClass.DigitalXRayImageStorageForPresentationUid,
                                 PresentationIntentType = "FOR PRESENTATION"
                             };

            var image2 = new ImageBuilder
                             {
                                 ImageLaterality = "U",
                                 ViewPosition = "LL",
                                 InstanceNumber = 1,
                                 SopClassUid = SopClass.DigitalXRayImageStorageForPresentationUid,
                                 PresentationIntentType = "FOR PRESENTATION"
                             };


            if (oneSeries)
            {
                var seriesBuilder = new SeriesBuilder
                                        {
                                            Modality = "DX",
                                            SeriesDescription = seriesDescription,
                                            SeriesNumber = startSeriesNumber,
                                            Images = new List<ImageBuilder> {image1 }
                                        };

                if (!oneImagePerSeries)
                    seriesBuilder.Images.Add(image2);

                studyBuilder.Series.Add(seriesBuilder);
            }
            else
            {
                var seriesBuilder = new SeriesBuilder
                                        {
                                            Modality = "DX",
                                            SeriesDescription = seriesDescription,
                                            SeriesNumber = startSeriesNumber,
                                            Images = new List<ImageBuilder> {image1}
                                        };

                studyBuilder.Series.Add(seriesBuilder);
                
                seriesBuilder = new SeriesBuilder
                                    {
                                        Modality = "DX",
                                        SeriesDescription = seriesDescription,
                                        SeriesNumber = startSeriesNumber,
                                        Images = new List<ImageBuilder> {image2}
                                    };

                studyBuilder.Series.Add(seriesBuilder);
            }

            return studyBuilder;
        }

        #endregion

        #region CR X-Ray

        // ReSharper disable InconsistentNaming
        public static StudyBuilder CreateCRXRayBuilder
// ReSharper restore InconsistentNaming
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string seriesDescription,
            int startSeriesNumber,
            bool oneImagePerSeries,
            bool createSingleImageDisplaySets
        )
        {
            return CreateCRXRayBuilder(patientId, patientsName, studyDescription, studyInstanceUid,
                                       seriesDescription, startSeriesNumber, !oneImagePerSeries,
                                       oneImagePerSeries, createSingleImageDisplaySets);
        }

// ReSharper disable InconsistentNaming
        public static StudyBuilder CreateCRXRayBuilder
// ReSharper restore InconsistentNaming
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string seriesDescription,
            int startSeriesNumber,
            bool oneSeries,
            bool oneImagePerSeries,
            bool createSingleImageDisplaySets
        )
        {
            if (!oneSeries && !oneImagePerSeries)
                throw new ArgumentException("Multiple Series currently restricted to one image each");

            var studyBuilder = new StudyBuilder
            {
                DisplaySetFactory = new BasicDisplaySetFactory { CreateSingleImageDisplaySets = createSingleImageDisplaySets },
                PatientId = patientId,
                PatientsName = patientsName,
                StudyDescription = studyDescription,
                StudyInstanceUid = studyInstanceUid
            };

            var image1 = new ImageBuilder
            {
                ImageLaterality = "U",
                ViewPosition = "AP",
                InstanceNumber = 1,
                SopClassUid = SopClass.ComputedRadiographyImageStorageUid
            };

            var image2 = new ImageBuilder
            {
                ImageLaterality = "U",
                ViewPosition = "LL",
                InstanceNumber = 1,
                SopClassUid = SopClass.ComputedRadiographyImageStorageUid
            };

            if (oneSeries)
            {
                var seriesBuilder = new SeriesBuilder
                                        {
                                            Modality = "CR",
                                            SeriesDescription = seriesDescription,
                                            SeriesNumber = startSeriesNumber,
                                            Images = new List<ImageBuilder> {image1}
                                        };

                if (!oneImagePerSeries)
                    seriesBuilder.Images.Add(image2);

                studyBuilder.Series.Add(seriesBuilder);
            }
            else
            {
                var seriesBuilder = new SeriesBuilder
                {
                    Modality = "CR",
                    SeriesDescription = seriesDescription,
                    SeriesNumber = startSeriesNumber,
                    Images = new List<ImageBuilder> { image1 }
                };

                studyBuilder.Series.Add(seriesBuilder);

                seriesBuilder = new SeriesBuilder
                {
                    Modality = "CR",
                    SeriesDescription = seriesDescription,
                    SeriesNumber = startSeriesNumber,
                    Images = new List<ImageBuilder> { image2 }
                };

                studyBuilder.Series.Add(seriesBuilder);
            }

            return studyBuilder;
        }

        #endregion

        #region Simple Chest CT

// ReSharper disable InconsistentNaming
        public static StudyBuilder CreateCTChestBuilder
// ReSharper restore InconsistentNaming
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string scoutSeriesDescription,
            string axialSeriesDescription,
            int startSeriesNumber
        )
        {
            return new StudyBuilder
                       {
                           PatientId = patientId,
                           PatientsName = patientsName,
                           StudyDescription = studyDescription,
                           StudyInstanceUid = studyInstanceUid,
                           Series = new List<SeriesBuilder>
                                        {
                                            #region Series
                                            new SeriesBuilder
                                                {
                                                    Modality = "CT",
                                                    SeriesDescription = scoutSeriesDescription,
                                                    SeriesNumber = startSeriesNumber,
                                                    Images = new List<ImageBuilder>
                                                                 {
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =ImageOrientationPatient.SaggittalAnterior.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 1,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.CoronalRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 2,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         }
                                                                 }
                                                },
                                            new SeriesBuilder
                                                {
                                                    Modality = "CT",
                                                    SeriesDescription = axialSeriesDescription,
                                                    SeriesNumber = startSeriesNumber + 1,
                                                    Images = new List<ImageBuilder>
                                                                 {
                                                                     //5 axials ought to be enough for testing.
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 1,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 2,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 3,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 4,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient = ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 5,
                                                                             SopClassUid = SopClass.CtImageStorageUid
                                                                         }
                                                                 }
                                                }
                                                #endregion
                                        }
                       };
        }

        #endregion

        #region Simple MR

        // ReSharper disable InconsistentNaming
        public static StudyBuilder CreateMREchoBuilder
        // ReSharper restore InconsistentNaming
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string series1Description,
            string series2Description,
            int startSeriesNumber,
            bool createEchoDisplaySets
        )
        {
            return CreateMRBuilder(patientId, patientsName, studyDescription, studyInstanceUid,
                                   series1Description, series2Description, startSeriesNumber, true, createEchoDisplaySets);
        }

        // ReSharper disable InconsistentNaming
        public static StudyBuilder CreateMRBuilder
            // ReSharper restore InconsistentNaming
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            string series1Description,
            string series2Description,
            int startSeriesNumber,
            bool createEchoSeries,
            bool createEchoDisplaySets
        )
        {
            return new StudyBuilder
                       {
                           DisplaySetFactory = createEchoDisplaySets 
                                ? (IDisplaySetFactory)new MREchoDisplaySetFactory() 
                                : new BasicDisplaySetFactory(),
                           PatientId = patientId,
                           PatientsName = patientsName,
                           StudyDescription = studyDescription,
                           StudyInstanceUid = studyInstanceUid,
                           Series = new List<SeriesBuilder>
                                        {
                                            #region Series

                                            new SeriesBuilder
                                                {
                                                    Modality = "MR",
                                                    SeriesDescription = series1Description,
                                                    SeriesNumber = startSeriesNumber,
                                                    Images = new List<ImageBuilder>
                                                                 {
                                                                     //5 axials ought to be enough for testing.
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 1,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 2,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 3,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 4,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 5,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.AxialRight.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 6,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         }
                                                                 }
                                                },
                                            new SeriesBuilder
                                                {
                                                    Modality = "MR",
                                                    SeriesDescription = series1Description,
                                                    SeriesNumber = startSeriesNumber + 1,
                                                    Images = new List<ImageBuilder>
                                                                 {
                                                                     //5 axials ought to be enough for testing.
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 1,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 2,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 3,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 4,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 5,
                                                                             EchoNumber = createEchoSeries ? 1 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         },
                                                                     new ImageBuilder
                                                                         {
                                                                             ImageOrientationPatient =
                                                                                 ImageOrientationPatient.CoronalLeft.ToString(),
                                                                             //Position doesn't matter right now as it's unused.
                                                                             ImagePositionPatient = @"0\0\0",
                                                                             InstanceNumber = 6,
                                                                             EchoNumber = createEchoSeries ? 2 : (int?)null,
                                                                             SopClassUid = SopClass.MrImageStorageUid
                                                                         }
                                                                 }
                                                }
                                            #endregion
                                        }
                       };
        }

        #endregion

        #region (Mixed) Multi-frame Ultrasound

        public static StudyBuilder CreateMixedMultiframeUltrasound
        (
            string patientId,
            string patientsName,
            string studyDescription,
            string studyInstanceUid,
            int seriesNumber,
            string seriesDescription,
            int numberSingleImages,
            int numberMultiframes,
            bool createSeriesDisplaySet,
            bool createSingleImageDisplaySets
        )
        {
            var studyBuilder = new StudyBuilder
                       {
                           PatientId = patientId,
                           PatientsName = patientsName,
                           StudyDescription = studyDescription,
                           StudyInstanceUid = studyInstanceUid
                       };

            if (numberMultiframes <= 0 && numberSingleImages <= 0)
                throw new ArgumentException();

            SeriesBuilder seriesBuilder;
            studyBuilder.Series.Add(seriesBuilder = new SeriesBuilder
            {
                SeriesDescription = seriesDescription,
                Modality = "US",
                SeriesNumber = seriesNumber
            });

            int instanceNumber = 1;
            for (int i = 0; i < numberSingleImages; i++, instanceNumber++)
            {
                seriesBuilder.Images.Add(new ImageBuilder
                                             {
                                                 InstanceNumber = instanceNumber,
                                                 SopClassUid = SopClass.UltrasoundImageStorageUid
                                             });
            }

            for (int i = 0; i < numberMultiframes; i++, instanceNumber++)
            {
                seriesBuilder.Images.Add(new ImageBuilder
                {
                    InstanceNumber = instanceNumber,
                    NumberOfFrames = 3,
                    SopClassUid = SopClass.UltrasoundMultiFrameImageStorageUid
                });
            }

            if (createSingleImageDisplaySets)
                studyBuilder.DisplaySetFactory = new BasicDisplaySetFactory{CreateSingleImageDisplaySets = true};
            //Only use the mixed multi-frame factory if it's going to work.
            else if (!createSeriesDisplaySet && (numberMultiframes > 1 || (numberMultiframes > 0 && numberSingleImages > 0)))
                studyBuilder.DisplaySetFactory = new MixedMultiFrameDisplaySetFactory();

            return studyBuilder;
        }

        #endregion
    }
}

#endif