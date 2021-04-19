using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SnowTopia_ModLoader
{
    class ModLoader
    {
        string originalAssembly = @"C:\Users\luukw\Desktop\modding\mod_loader\Assembly-CSharp-original.dll";
        string output = "";

        public ModLoader(string output)
        {
            this.output = output;
        }

        public void LoadMods(List<Mod> mods)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD originalModule = ModuleDefMD.Load(originalAssembly, modCtx);

            // Restore Original
            originalModule.Write(output);

            mods.ForEach(mod =>
            {
                if (mod.Active)
                {
                    var jsonString = File.ReadAllText(mod.Path);
                    Save save = JsonConvert.DeserializeObject<Save>(jsonString);

                    InjectNewClasses(save, originalModule);

                    InjectModdedMethods(save, originalModule);

                    originalModule.Write(output);
                }
            });
        }

        private void InjectNewClasses(Save save, ModuleDefMD originalModule)
        {
            save.NewClasses.ForEach(newClass =>
            {
                var className = Helpers.GetClassFromFullName(newClass.FullName);

                var newType = new TypeDefUser(className,
                                originalModule.CorLibTypes.Object.TypeDefOrRef);

                newType.Attributes = (dnlib.DotNet.TypeAttributes)Helpers.GetTypeAttributes(newClass.Attributes);

                originalModule.Types.Add(newType);

                newClass.Fields.ForEach(field =>
                {
                    var fieldName = Helpers.GetNameFromFullName(field.FullName);
                    var fieldTypeNamespace = Helpers.GetNameSpaceOfTypeFromFullName(field.FullName);
                    var fieldType = Helpers.GetTypeFromFullName(field.FullName);
                    var fieldSig = new FieldSig(originalModule.CorLibTypes.GetTypeRef(fieldTypeNamespace, fieldType).ToTypeSig());
                    var attributes = (FieldAttributes)Helpers.GetFieldAttributes(field.Attributes);

                    var newField = new FieldDefUser(fieldName, fieldSig, attributes);

                    newType.Fields.Add(newField);
                });

                newClass.Methods.ForEach(method =>
                {
                    var methodName = Helpers.GetNameFromFullName(method.FullName);
                    var methodImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                    var methodFlags = (MethodAttributes)Helpers.GetMethodAttributes(method.Attributes);
                    

                });
            });
        }

        private void InjectModdedMethods(Save save, ModuleDefMD originalModule)
        {
            save.ModdedMethods.ForEach(method =>
            {
                var className = Helpers.GetClassFromFullName(method.FullName);
                var methodDef = GetMethod(originalModule, className, method.FullName);
                var newInstructionSet = new List<Instruction>();
                var insertingMod = false;
                var i = 0;

                foreach (var originalInstruction in methodDef.Body.Instructions)
                {
                    // Find Start
                    if (originalInstruction.Offset == Helpers.ConvertOffsetToInt(method.Start))
                    {
                        if(method.Start != "")
                            newInstructionSet.Add(originalInstruction);

                        insertingMod = true;
                        var j = i;

                        foreach (var instructionString in method.Instructions)
                        {
                            var newInstruction = CreateInstruction(originalModule, instructionString);
                            newInstructionSet.Add(newInstruction);

                            j++;
                        }
                    }

                    if (!insertingMod)
                        newInstructionSet.Add(originalInstruction);

                    if (originalInstruction.Offset == Helpers.ConvertOffsetToInt(method.End))
                    {
                        newInstructionSet.Add(originalInstruction);

                        insertingMod = false;
                    }

                    i++;
                }

                methodDef.Body.Instructions.Clear();

                newInstructionSet.ForEach(instruction =>
                {
                    methodDef.Body.Instructions.Add(instruction);
                });

            });
        }

        private MethodDef GetMethod(ModuleDefMD module, string className, string FullMethodName)
        {
            var classDef = module.Find(className, true);
            var methodDef = classDef.Methods.Where(x => x.FullName == FullMethodName).FirstOrDefault();
            return methodDef;
        }

        private Instruction CreateInstruction(ModuleDefMD module, string instructionString)
        {
            Regex regex = new Regex("IL_(?<offset>.+): (?<opcode>[^ ]+)( (?<oprand>.+))?");
            var match = regex.Match(instructionString);

            var offset = match.Groups["offset"].Value;
            var opCodeString = match.Groups["opcode"].Value;
            var opRandString = match.Groups["oprand"].Value;

            var opCode = typeof(OpCodes)
                    .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .Where(x => x.FieldType == typeof(OpCode))
                    .Select(x => (OpCode)x.GetValue(null))
                    .Where(x => x.ToString() == opCodeString)
                    .FirstOrDefault();

            int n;

            if(opRandString == "")
            {
                return opCode.ToInstruction();
            } 
            else if(int.TryParse(opRandString, out n))
            {
                return opCode.ToInstruction((sbyte)n);
            }
            else
            {
                foreach (var type in module.GetTypes())
                {
                    
                    if (type.Fields.Any(x => x.FullName == opRandString))
                    {
                        var operand = type.Fields.Where(x => x.FullName == opRandString).FirstOrDefault();
                        return opCode.ToInstruction(operand);
                    }
                    
                }

            }

            return new Instruction();
        }
    }


}
