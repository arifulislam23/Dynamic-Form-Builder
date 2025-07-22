using Dynamic_Form_Builder.Interface;
using Dynamic_Form_Builder.ViewModel;
using Microsoft.Data.SqlClient;

namespace Dynamic_Form_Builder.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly string _connectionString;

        public FormRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

       
        public int SaveForm(FormVM form)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var insertFormCmd = new SqlCommand("INSERT INTO Forms (Title) OUTPUT INSERTED.FormId VALUES (@Title)", conn);
            insertFormCmd.Parameters.AddWithValue("@Title", form.Title);
            int newFormId = (int)insertFormCmd.ExecuteScalar();

            foreach (var field in form.Fields)
            {
                var insertFieldCmd = new SqlCommand(
                    "INSERT INTO FormFields (FormId, Label, IsRequired, SelectedOption) VALUES (@FormId, @Label, @IsRequired, @SelectedOption)", conn);

                insertFieldCmd.Parameters.AddWithValue("@FormId", newFormId);
                insertFieldCmd.Parameters.AddWithValue("@Label", field.Label ?? string.Empty);
                insertFieldCmd.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                insertFieldCmd.Parameters.AddWithValue("@SelectedOption", field.SelectedOption ?? string.Empty);

                insertFieldCmd.ExecuteNonQuery();
            }

            return newFormId;
        }

        public List<FormVM> GetAllForms()
        {
            var forms = new List<FormVM>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT FormId, Title FROM Forms ORDER BY FormId DESC", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                forms.Add(new FormVM
                {
                    FormId = (int)reader["FormId"],
                    Title = reader["Title"].ToString()
                });
            }

            return forms;
        }

        public FormVM GetFormWithFields(int formId)
        {
            var form = new FormVM();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var formCmd = new SqlCommand("SELECT Title FROM Forms WHERE FormId = @Id", conn);
            formCmd.Parameters.AddWithValue("@Id", formId);
            form.Title = (string)formCmd.ExecuteScalar();

            var fieldsCmd = new SqlCommand("SELECT FieldId, Label, IsRequired, SelectedOption FROM FormFields WHERE FormId = @FormId", conn);
            fieldsCmd.Parameters.AddWithValue("@FormId", formId);
            using var reader = fieldsCmd.ExecuteReader();

            form.Fields = new List<FormFieldVM>();
            while (reader.Read())
            {
                form.Fields.Add(new FormFieldVM
                {
                    FieldId = (int)reader["FieldId"],
                    Label = reader["Label"].ToString(),
                    IsRequired = (bool)reader["IsRequired"],
                    SelectedOption = reader["SelectedOption"].ToString()
                });
            }

            return form;
        }

        public bool UpdateFormFields(FormVM form, out string error)
        {
            error = "";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                foreach (var field in form.Fields)
                {
                    var cmd = new SqlCommand("UPDATE FormFields SET SelectedOption = @SelectedOption WHERE FieldId = @FieldId", conn);
                    cmd.Parameters.AddWithValue("@SelectedOption", field.SelectedOption ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
