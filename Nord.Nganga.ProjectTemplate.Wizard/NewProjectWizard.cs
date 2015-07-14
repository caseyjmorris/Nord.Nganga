using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TemplateWizard;

namespace Nord.Nganga.ProjectTemplate.Wizard
{
    public class NewProjectWizard : IWizard
    {
      public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
      {
        
      }

      public void ProjectFinishedGenerating(EnvDTE.Project project)
      {
        
      }

      public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
      {
        
      }

      public void RunFinished()
      {
        MessageBox.Show("Your new Nganga client project has been created - thank you for using Nganga!");
      }

      public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
      {
        try
        {
          var inputForm = new UserInputForm(replacementsDictionary);
          inputForm.ShowDialog();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString());
        }
      }

      public bool ShouldAddProjectItem(string filePath)
      {
        return true;
      }
    }
}
