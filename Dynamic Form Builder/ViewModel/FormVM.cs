namespace Dynamic_Form_Builder.ViewModel
{
    public class FormVM
    {
        public int FormId { get; set; }
        public string? Title { get; set; }
        public List<FormFieldVM> Fields { get; set; } = new List<FormFieldVM>();
    }

}
