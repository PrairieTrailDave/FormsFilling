using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


using FormFilling.Models;
using FormFilling.Services;

namespace FormFilling.Services
{
    public class FormsService : IFormsService
    {
        public string ErrorMessage { get; set; } = "";

        // injected variables

        private readonly DatabaseContext _context;



        public FormsService(
            DatabaseContext context
            )
        {
            _context = context;
        }

        public async Task<FormEditView> GetFormView(string FormID)
        {
            FormEditView tFormView;
            string ThisFormCode;

            tFormView = new FormEditView();

            // pull the form again to get the other variables. This way, the API can pull this just with a form ID

            Form? tForm = await _context.Forms.FirstOrDefaultAsync(m => m.FormCode == FormID);
            if (tForm == null) return tFormView;
            ThisFormCode = tForm.FormCode;

            // get all the parts of this form

            List<FormItem> tFormItems = await _context.FormItems.Where(m => m.FormCode == FormID).OrderBy(n => n.DisplayOrder).ToListAsync();

            tFormView.ID = tForm.ID;
            tFormView.FormCode = ThisFormCode;
            tFormView.VersionID = tForm.VersionID;
            tFormView.FormPage = tForm.FormPage ?? "";
            tFormView.FormDescription = tForm.FormDescription ?? "";
            tFormView.ListOfFormItems = new List<FormViewItem>();

            if (tFormItems.Count > 0)
            {

                // build the form view

                foreach (FormItem fr in tFormItems)
                {
                    switch (fr.ItemType)
                    {
                        case FormItemTypes.ItemTypePrompt:
                            FormViewItem FP = new()
                            {
                                TypeOfFormItem = FormViewItem.FormItemType.Label.ToString(),
                                FieldName = fr.ItemName,

                                // get the prompts for this item

                                Prompt = new PromptItem[2]
                            };
                            List<FormPrompt> Prompts = await _context.FormPrompts.Where(Fp =>
                                                                           (Fp.FormCode == ThisFormCode) &&
                                                                           (Fp.LabelName == fr.ItemName)).OrderBy(p => p.Language).ToListAsync();
                            for (int i = 0; i < Prompts.Count; i++)
                            {
                                PromptItem PI = new()
                                {
                                    Language = Prompts[i].Language,
                                    PromptValue = Prompts[i].Prompt ?? ""
                                };
                                FP.Prompt[i] = PI;
                            }
                            FP.DisplayPrompt = StrMax(Prompts[0].Prompt ?? "", 30);
                            FP.DisplayFormattingClass = fr.DisplayFormatting;
                            FP.InputFormattingClass = fr.DisplayFormatting;
                            tFormView.ListOfFormItems.Add(FP);
                            break;
                        case FormItemTypes.ItemTypeInput:
                            FormViewItem FI = new()
                            {
                                TypeOfFormItem = FormViewItem.FormItemType.Input.ToString()
                            };

                            // Input type can be:
                            //                 A - Address
                            //                 B - checkBox
                            //                 C - City state zip
                            //                 D - Decimal
                            //                 E - datE
                            //                 F - true/False
                            //                 L - Last name
                            //                 M - Middle name
                            //                 N - Number
                            //                 p - Phone
                            //                 R - fiRst name
                            //                 S - Ssn
                            //                 T - Text 
                            //

                            switch (fr.InputType)
                            {
                                case FormInputTypes.Address:
                                    FI.InputType = FormViewItem.FieldInputTypes.Address; break;
                                case FormInputTypes.CheckBox:
                                    FI.InputType = FormViewItem.FieldInputTypes.CheckBox; break;
                                case FormInputTypes.CityState:
                                    FI.InputType = FormViewItem.FieldInputTypes.CityState; break;
                                case FormInputTypes.Decimal:
                                    FI.InputType = FormViewItem.FieldInputTypes.Decimal; break;
                                case FormInputTypes.Date:
                                    FI.InputType = FormViewItem.FieldInputTypes.Date; break;
                                case FormInputTypes.TrueFalse:
                                    FI.InputType = FormViewItem.FieldInputTypes.TrueFalse; break;
                                case FormInputTypes.LastName:
                                    FI.InputType = FormViewItem.FieldInputTypes.LastName; break;
                                case FormInputTypes.MiddleName:
                                    FI.InputType = FormViewItem.FieldInputTypes.MiddleName; break;
                                case FormInputTypes.Number:
                                    FI.InputType = FormViewItem.FieldInputTypes.Number; break;
                                case FormInputTypes.Phone:
                                    FI.InputType = FormViewItem.FieldInputTypes.Phone; break;
                                case FormInputTypes.FirstName:
                                    FI.InputType = FormViewItem.FieldInputTypes.FirstName; break;
                                case FormInputTypes.SSN:
                                    FI.InputType = FormViewItem.FieldInputTypes.SSN; break;
                                case FormInputTypes.Text:
                                    FI.InputType = FormViewItem.FieldInputTypes.Text; break;
                            }
                            FI.FieldName = fr.ItemName;

                            // get the prompts for this item

                            FI.Prompt = new PromptItem[2];
                            List<FormPrompt> IPrompts = await _context.FormPrompts.Where(Fp =>
                                                                           (Fp.FormCode == ThisFormCode) &&
                                                                           (Fp.LabelName == fr.ItemName)).OrderBy(p => p.Language).ToListAsync();
                            for (int i = 0; i < IPrompts.Count; i++)
                            {
                                PromptItem PI = new()
                                {
                                    Language = IPrompts[i].Language,
                                    PromptValue = IPrompts[i].Prompt ?? ""
                                };
                                FI.Prompt[i] = PI;
                            }
                            FI.DisplayPrompt = (IPrompts[0].Prompt ?? "").Substring(0, Math.Min(30, (IPrompts[0].Prompt ?? "").Length));

                            FI.DisplayFormattingClass = fr.DisplayFormatting;
                            FI.InputFormattingClass = fr.InputFormatting ?? "";
                            tFormView.ListOfFormItems.Add(FI);
                            break;
                            // need footer
                    }
                }
            }
            return tFormView;
        }


        // what is different between this and the previous one? 
        // the overall structure returned is different (FormView vs. FormEditView)

        public async Task<bool> GetFormItems(FormView tFormView, string searchProviderCode, string searchEmployerCode, string FormCode)
        {
            tFormView.ListOfFormItems = new List<FormViewItem>();

            // get the elements of this form

            List<FormItem> tFormItems = await _context.FormItems.Where(fm => fm.FormCode == FormCode).OrderBy(ob => ob.DisplayOrder).ToListAsync();

            // build the form view

            foreach (FormItem fr in tFormItems)
            {
                switch (fr.ItemType)
                {
                    case FormItemTypes.ItemTypeHeader:
                        tFormView.Header = fr.ItemName;
                        break;
                    case FormItemTypes.ItemTypeInstructions:
                        tFormView.Instructions = fr.ItemName;
                        break;
                    case FormItemTypes.ItemTypePrompt:
                        FormViewItem FP = new()
                        {
                            TypeOfFormItem = FormViewItem.FormItemType.Label.ToString(),
                            FieldName = fr.ItemName,
                            InputType = "",

                            // get the prompts for this item
                            //  check first for employer specific, then provider specific, then in general (blank)

                            Prompt = new PromptItem[2]    // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  right now, allowing only two languages on a page
                        };

                        List<FormPrompt> Prompts = await _context.FormPrompts.Where(Fp =>
                                                                       (Fp.FormCode == FormCode) &&
                                                                       (Fp.LabelName == fr.ItemName)).OrderBy(p => p.Language).ToListAsync();


                        for (int i = 0; i < Prompts.Count; i++)
                        {
                            PromptItem PI = new()
                            {
                                Language = Prompts[i].Language,
                                PromptValue = Prompts[i].Prompt ?? ""
                            };
                            FP.Prompt[i] = PI;
                        }
                        FP.DisplayFormattingClass = fr.DisplayFormatting;
                        tFormView.ListOfFormItems.Add(FP);
                        break;
                    case FormItemTypes.ItemTypeInput:
                        FormViewItem FI = new()
                        {
                            TypeOfFormItem = FormViewItem.FormItemType.Input.ToString()
                        };

                        switch (fr.InputType)
                        {
                            case FormInputTypes.Address:
                                FI.InputType = FormViewItem.FieldInputTypes.Address; break;
                            case FormInputTypes.CheckBox:
                                FI.InputType = FormViewItem.FieldInputTypes.CheckBox; break;
                            case FormInputTypes.CityState:
                                FI.InputType = FormViewItem.FieldInputTypes.CityState; break;
                            case FormInputTypes.Decimal:
                                FI.InputType = FormViewItem.FieldInputTypes.Decimal; break;
                            case FormInputTypes.Date:
                                FI.InputType = FormViewItem.FieldInputTypes.Date; break;
                            case FormInputTypes.TrueFalse:
                                FI.InputType = FormViewItem.FieldInputTypes.TrueFalse; break;
                            case FormInputTypes.LastName:
                                FI.InputType = FormViewItem.FieldInputTypes.LastName; break;
                            case FormInputTypes.MiddleName:
                                FI.InputType = FormViewItem.FieldInputTypes.MiddleName; break;
                            case FormInputTypes.Number:
                                FI.InputType = FormViewItem.FieldInputTypes.Number; break;
                            case FormInputTypes.Phone:
                                FI.InputType = FormViewItem.FieldInputTypes.Phone; break;
                            case FormInputTypes.FirstName:
                                FI.InputType = FormViewItem.FieldInputTypes.FirstName; break;
                            case FormInputTypes.SSN:
                                FI.InputType = FormViewItem.FieldInputTypes.SSN; break;
                            case FormInputTypes.Text:
                                FI.InputType = FormViewItem.FieldInputTypes.Text; break;
                        }
                        FI.FieldName = fr.ItemName;

                        // get the prompts for this item

                        FI.Prompt = new PromptItem[2];
                        List<FormPrompt> IPrompts = await _context.FormPrompts.Where(Fp =>
                                                                       (Fp.FormCode == FormCode) &&
                                                                       (Fp.LabelName == fr.ItemName)).OrderBy(p => p.Language).ToListAsync();
                        for (int i = 0; i < IPrompts.Count; i++)
                        {
                            PromptItem PI = new()
                            {
                                Language = IPrompts[i].Language,
                                PromptValue = IPrompts[i].Prompt ?? ""
                            };
                            FI.Prompt[i] = PI;
                        }
                        FI.DisplayFormattingClass = fr.DisplayFormatting;

                        FI.InputFormattingClass = fr.InputFormatting ?? "";
                        tFormView.ListOfFormItems.Add(FI);
                        break;
                        // need footer
                }
            }
            return true;
        }


        public void FillInEmployeeInformation(FormView tFormView, Employee TEmployee)
        {

            // fill in the person's information
            foreach (FormViewItem FVI in tFormView.ListOfFormItems)
            {
                if (FVI.TypeOfFormItem == FormViewItem.FormItemType.Input.ToString())
                {
                    switch (FVI.InputType)
                    {
                        case FormViewItem.FieldInputTypes.Address:
                            FVI.CurrentValue = TEmployee.EmployeeAddress1;
                            break;
                        case FormViewItem.FieldInputTypes.CheckBox:
                            break;
                        case FormViewItem.FieldInputTypes.CityState:
                            FVI.CurrentValue = TEmployee.EmployeeCity + ", " + TEmployee.EmployeeState + " " + TEmployee.EmployeeZip;
                            break;
                        case FormViewItem.FieldInputTypes.FirstName:
                            FVI.CurrentValue = TEmployee.FirstName;
                            break;
                        case FormViewItem.FieldInputTypes.LastName:
                            FVI.CurrentValue = TEmployee.LastName;
                            break;
                        case FormViewItem.FieldInputTypes.MiddleName:
                            FVI.CurrentValue = TEmployee.MiddleName;
                            break;
                        case FormViewItem.FieldInputTypes.Phone:
                            break;
                        case FormViewItem.FieldInputTypes.SSN:
                            FVI.CurrentValue = TEmployee.SSN;
                            break;
                    }
                }
            }

        }


        public bool ValidateForm(FormEditView tView)
        {
            ErrorMessage = "";
            if (Required(tView.FormCode, "Form Code")) return true;
            if (ValidateField(tView.FormCode, Form.FormCodeLength, "Form Code")) return true;


            foreach (FormViewItem tv in tView.ListOfFormItems)
            {
                if (Required(tv.FieldName, "Item Name")) return true;
                if (ValidateField(tv.FieldName, FormItem.ItemNameLength, "Item Name")) return true;
                if (ValidateField(tv.DisplayFormattingClass, FormItem.DisplayFormatingLength, tv.FieldName + " Display Formatting")) return true;
                if (ValidateField(tv.InputFormattingClass, FormItem.InputFormattingLength, tv.FieldName + " Input Formatting")) return true;
                if (Required(tv.Prompt[0].Language, "Prompt Language")) return true;
                if (ValidateField(tv.Prompt[0].Language, FormPrompt.LanguageLength, tv.FieldName + " Prompt Language")) return true;
                if (Required(tv.Prompt[1].Language, "Prompt Language")) return true;
                if (ValidateField(tv.Prompt[1].Language, FormPrompt.LanguageLength, tv.FieldName + " Prompt Language")) return true;
                if (ValidateField(tv.Prompt[0].PromptValue, FormPrompt.PromptLength, tv.FieldName + " Prompt")) return true;
                if (ValidateField(tv.Prompt[1].PromptValue, FormPrompt.PromptLength, tv.FieldName + " Prompt")) return true;
            }
            return false;
        }

        private bool ValidateField(string? str, int len, string Name)
        {
            if (str == null) return false;
            if (str.Length > len)
            {
                ErrorMessage = Name + " is too long " + str.Length.ToString() + " chars";
                return true;
            }

            return false;
        }
        private bool Required(string str, string Name)
        {
            ErrorMessage = Name + " is required and is missing";
            if (str == null) return true;
            if (str.Length == 0) return true;
            if (str.Trim().Length == 0) return true;
            return false;
        }

        public async Task<Form> CreateForm(FormEditView tView)
        {
            Form tForm = new()
            {
                FormCode = StrMax(tView.FormCode, Form.FormCodeLength),
                FormDescription = StrMax(tView.FormDescription, Form.DescriptionLength),
                FormPage = StrMax(tView.FormPage, Form.PageCodeLength),
                VersionDate = DateTime.Now,
                VersionID = 1
            };

            _context.Forms.Add(tForm);
            await _context.SaveChangesAsync();

            // get the id for this new form

            int nFormID = tForm.ID;
            await SaveFieldItems(tView, tForm);

            return tForm;
        }


        public async Task<Form> UpdateForm(FormEditView tView)
        {
            Form? tForm = await _context.Forms.FirstOrDefaultAsync(m => m.ID == tView.ID);
            if (tForm != null)
            {
                tForm.FormCode = StrMax(tView.FormCode, Form.FormCodeLength);
                tForm.FormPage = StrMax(tView.FormPage, Form.PageCodeLength);
                tForm.VersionDate = DateTime.Now;
                tForm.VersionID = tForm.VersionID + 1;

                await _context.SaveChangesAsync();



                // may want to delete any existing items.
                List<FormItem> tFormItems = await _context.FormItems.Where(m => m.FormCode == tView.FormCode).OrderBy(n => n.DisplayOrder).ToListAsync();
                foreach (FormItem fi in tFormItems)
                {
                    List<FormPrompt> Prompts = await _context.FormPrompts.Where(Fp =>
                                                   (Fp.FormCode == tForm.FormCode) &&
                                                   (Fp.LabelName == fi.ItemName)).ToListAsync();
                    foreach (FormPrompt P in Prompts)
                        _context.FormPrompts.Remove(P);
                    _context.FormItems.Remove(fi);
                }
                await _context.SaveChangesAsync();

                await SaveFieldItems(tView, tForm);
            }
            return tForm!;
        }

        private async Task<bool> SaveFieldItems(FormEditView tView, Form tForm)
        {
            // add in all the items and prompts from the view
            int DisplayOrder = 0;

            foreach (FormViewItem tv in tView.ListOfFormItems)
            {
                FormItem tFormItem = new()
                {
                    FormCode = tForm.FormCode
                };
                switch (tv.TypeOfFormItem)
                {
                    case "Label": tFormItem.ItemType = FormItemTypes.ItemTypePrompt; break;
                    case "Input": tFormItem.ItemType = FormItemTypes.ItemTypeInput; break;
                }

                tFormItem.DisplayFormatting = tv.DisplayFormattingClass ?? "";
                tFormItem.DisplayOrder = DisplayOrder;
                DisplayOrder++;

                tFormItem.ItemName = tv.FieldName;
                tFormItem.InputType = "";
                switch (tv.InputType)
                {
                    case FormViewItem.FieldInputTypes.Address:
                        tFormItem.InputType = FormInputTypes.Address; break;
                    case FormViewItem.FieldInputTypes.CheckBox:
                        tFormItem.InputType = FormInputTypes.CheckBox; break;
                    case FormViewItem.FieldInputTypes.CityState:
                        tFormItem.InputType = FormInputTypes.CityState; break;
                    case FormViewItem.FieldInputTypes.Decimal:
                        tFormItem.InputType = FormInputTypes.Decimal; break;
                    case FormViewItem.FieldInputTypes.Date:
                        tFormItem.InputType = FormInputTypes.Date; break;
                    case FormViewItem.FieldInputTypes.TrueFalse:
                        tFormItem.InputType = FormInputTypes.TrueFalse; break;
                    case FormViewItem.FieldInputTypes.LastName:
                        tFormItem.InputType = FormInputTypes.LastName; break;
                    case FormViewItem.FieldInputTypes.MiddleName:
                        tFormItem.InputType = FormInputTypes.MiddleName; break;
                    case FormViewItem.FieldInputTypes.Number:
                        tFormItem.InputType = FormInputTypes.Number; break;
                    case FormViewItem.FieldInputTypes.Phone:
                        tFormItem.InputType = FormInputTypes.Phone; break;
                    case FormViewItem.FieldInputTypes.FirstName:
                        tFormItem.InputType = FormInputTypes.FirstName; break;
                    case FormViewItem.FieldInputTypes.SSN:
                        tFormItem.InputType = FormInputTypes.SSN; break;
                    case FormViewItem.FieldInputTypes.Text:
                        tFormItem.InputType = FormInputTypes.Text; break;
                }
                tFormItem.InputFormatting = tv.InputFormattingClass;

                _context.FormItems.Add(tFormItem);


                // need to add the prompts

                if (tv.Prompt[0].PromptValue != "")
                {
                    FormPrompt P0 = new()
                    {
                        FormCode = tForm.FormCode,
                        LabelName = tv.FieldName,
                        Language = StrMax(tv.Prompt[0].Language, FormPrompt.LanguageLength),
                        Prompt = StrMax(tv.Prompt[0].PromptValue, FormPrompt.PromptLength),
                    };
                    _context.FormPrompts.Add(P0);
                }

                if (tv.Prompt[1].PromptValue != "")
                {
                    FormPrompt P1 = new()
                    {
                        FormCode = tForm.FormCode,
                        LabelName = tv.FieldName,
                        Language = StrMax(tv.Prompt[1].Language, FormPrompt.LanguageLength),
                        Prompt = StrMax(tv.Prompt[1].PromptValue, FormPrompt.PromptLength)
                    };
                    _context.FormPrompts.Add(P1);
                }
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FormExists(FormEditView tView)
        {

            string testFormCode = tView.FormCode;

            Form? tF = await (from tf in _context.Forms
                             where tf.FormCode == testFormCode 
                             select tf).FirstOrDefaultAsync();

            if (tF == null) return false;
            return true;
        }

        private static string StrMax(string? str, int max)
        {
            if (str == null) return "";
            return str.Substring(0, Math.Min(max, str.Length));
        }
    }
}