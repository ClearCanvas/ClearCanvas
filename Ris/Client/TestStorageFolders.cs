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
using System.Text;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class TestStorageFolders
    {
        public class Dog
        {
            public string Name;
            public string Breed;
            public string Color;
            public int Age;

            public Dog(string name, string breed, string color, int age)
            {
                Name = name;
                Breed = breed;
                Color = color;
                Age = age;
            }
        }

        public class Cat
        {
            public string Name;
            public string Breed;
            public string Color;
            public int Age;

            public Cat(string name, string breed, string color, int age)
            {
                Name = name;
                Breed = breed;
                Color = color;
                Age = age;
            }
        }


        public class DogTable : Table<Dog>
        {
            public DogTable()
            {
                this.Columns.Add(new TableColumn<Dog, string>("Name", delegate(Dog d) { return d.Name; }));
                this.Columns.Add(new TableColumn<Dog, string>("Breed", delegate(Dog d) { return d.Breed; }));
                this.Columns.Add(new TableColumn<Dog, string>("Color", delegate(Dog d) { return d.Color; }));
                this.Columns.Add(new TableColumn<Dog, string>("Age", delegate(Dog d) { return d.Age.ToString(); }));
            }
        }

        public class CatTable : Table<Cat>
        {
            public CatTable()
            {
                this.Columns.Add(new TableColumn<Cat, string>("Name", delegate(Cat d) { return d.Name; }));
                this.Columns.Add(new TableColumn<Cat, string>("Breed", delegate(Cat d) { return d.Breed; }));
                this.Columns.Add(new TableColumn<Cat, string>("Color", delegate(Cat d) { return d.Color; }));
                this.Columns.Add(new TableColumn<Cat, string>("Age", delegate(Cat d) { return d.Age.ToString(); }));
            }
        }


        public class DogFolder : StorageFolder<Dog>
        {
            public DogFolder(string folderName)
                :base(folderName, new DogTable())
            {
            }
        }

        public class CatFolder : StorageFolder<Cat>
        {
            public CatFolder(string folderName)
                : base(folderName, new CatTable())
            {
            }
        }

        public class MyDogsFolder : DogFolder
        {
            public MyDogsFolder()
                :base("My Dogs")
            {
                this.Items.Add(new Dog("Big Ben", "Terrier", "Brown", 2));
                this.Items.Add(new Dog("Marvin", "Pug", "White", 6));
                this.Items.Add(new Dog("Beauty", "Poodle", "Black", 3));
            }
        }

        public class LostDogsFolder : DogFolder
        {
            public LostDogsFolder()
                :base("Lost Dogs")
            {

            }
        }

        public class FoundDogsFolder : DogFolder
        {
            public FoundDogsFolder()
                : base("Found Dogs")
            {

            }
        }

        public class CryingCatsFolder : CatFolder
        {
            public CryingCatsFolder()
                :base("Crying Cats")
            {
            }
        }

        public class FriendlyCatsFolder : CatFolder
        {
            public FriendlyCatsFolder()
                :base("Friendly Cats")
            {

            }
        }

        public class StrayCatsFolder : CatFolder
        {
            public StrayCatsFolder()
                :base("Stray Cats")
            {
                this.Items.Add(new Cat("Smokey", "Unknown", "Brown", 4));
                this.Items.Add(new Cat("Blue Eyes", "Unknown", "White", 1));
                this.Items.Add(new Cat("Kat", "Siamese", "Black", 6));
            }
        }

    }
}
