using Database.Afrobarometer.Inputs;

namespace XycloneDesigns.Apis.Afrobarometer.Tables
{
    public static class QuestionExtensions 
    {
		public static Question FromCodebookQuestion(this Question question, CodebookPDF.Question codebookpdfquestion)
		{
			question.Id = codebookpdfquestion.Id;
			question.VariableLabel = codebookpdfquestion.VariableLabel;
			question.Text = codebookpdfquestion.Text;
			question.Source = codebookpdfquestion.Source;
			question.Note = codebookpdfquestion.Note;

			return question;
		}
	}
}