using Dynamic_Form_Builder.ViewModel;

namespace Dynamic_Form_Builder.Interface
{
    public interface IFormRepository
    {
        int SaveForm(FormVM form);
        List<FormVM> GetAllForms();
        FormVM GetFormWithFields(int formId);
        bool UpdateFormFields(FormVM form, out string error);
    }
}
