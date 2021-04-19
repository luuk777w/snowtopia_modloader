using System;
using System.Collections.Generic;
using System.Text;

namespace SnowTopia_ModLoader
{
    class Save
    {
        public List<ModdedMethodSave> ModdedMethods = new List<ModdedMethodSave>();

        public List<NewMethodSave> NewMethods = new List<NewMethodSave>();

        public List<NewFieldSave> NewFields = new List<NewFieldSave>();

        public List<NewClassSave> NewClasses = new List<NewClassSave>();
    }

    class ModdedMethodSave
    {
        public string FullName { get; set; }

        public string Start { get; set; }

        public List<string> Instructions = new List<string>();

        public string End { get; set; }
    }

    class NewMethodSave
    {
        public string FullName { get; set; }

        public string Attributes { get; set; }

        public List<string> Instructions = new List<string>();
    }

    class NewFieldSave
    {
        public string FullName { get; set; }

        public string Attributes { get; set; }
    }

    class NewClassSave
    {
        public string FullName { get; set; }

        public string BaseType { get; set; }

        public string Attributes { get; set; }

        public List<NewFieldSave> Fields = new List<NewFieldSave>();

        public List<NewMethodSave> Methods = new List<NewMethodSave>();
    }
}
