namespace Dynamic_Form_Builder.ViewModel
{
    public class FormFieldVM
    {
        public int FieldId { get; set; }
        public int FormId { get; set; }
        public string? Label { get; set; }
        public bool IsRequired { get; set; }
        public string? SelectedOption { get; set; }
    }
}
