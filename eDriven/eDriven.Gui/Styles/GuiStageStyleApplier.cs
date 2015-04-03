using System;
using eDriven.Gui.Containers;

namespace eDriven.Gui.Styles
{
    internal class GuiStageStyleApplier : IStyleApplyer
    {
        public void Apply()
        {
            foreach (var st in StageManager.Instance.StageList)
            {
                InitStageStyles(st);
            }
        }

        public static void InitStageStyles(Stage stage)
        {
            //LogUtil.PrintCurrentMethod();
            try
            {
                stage.RegenerateStyleCache(true);
                stage.StyleChanged(null);
                stage.NotifyStyleChangeInChildren(null, null, true);
                stage.StylesInitialized();
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex.Message);
                throw new Exception("Couldn't init Stage styles for stage: " + stage, ex);
            }
        }
    }
}
