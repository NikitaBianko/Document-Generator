using System.Text;
using HandlebarsDotNet;

namespace DocumentGenerator.Core
{
    public interface IDocumentTemplate
    {
        Document Generate(object context);
    }
    
    
    public class HandlebarsDocumentTemplate: IDocumentTemplate
    {
        private readonly string templateText;
        private readonly HandlebarsTemplate<object,object> generatorFunc;

        public HandlebarsDocumentTemplate(string templateText)
        {
            this.templateText = templateText;
            this.generatorFunc = Handlebars.Compile(templateText);
        }
        
        public Document Generate(object context)
        {
            var htmlContent = this.generatorFunc(context);

            return new Document(
                "text/html",
                Encoding.Default.GetBytes(htmlContent)
            );
        }
    }

}