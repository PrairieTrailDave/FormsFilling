using System.Threading.Tasks;

using FormFilling.Models;



namespace FormFilling.Services
{
    public interface IFormsService
    {
        Task<Form> CreateForm(FormEditView tView);
        void FillInEmployeeInformation(FormView tFormView, Employee TEmployee);
        Task<bool> FormExists(FormEditView tView);
        Task<bool> GetFormItems(FormView tFormView, string searchProviderCode, string searchEmployerCode, string FormCode);
        Task<FormEditView> GetFormView(string FormID);
        Task<Form> UpdateForm(FormEditView tView);
        bool ValidateForm(FormEditView tView);
    }
}