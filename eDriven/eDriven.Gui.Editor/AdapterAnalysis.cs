#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using eDriven.Gui.Designer.Adapters;

namespace eDriven.Gui.Editor
{
    /// <summary>
    /// The class analyzing the selected component adapter (on game object selection change)
    /// </summary>
    internal class AdapterAnalysis
    {
        public ComponentAdapter ComponentAdapter;
        public SkinnableComponentAdapter SkinnableComponentAdapter;
        public SkinnableContainerAdapter SkinnableContainerAdapter;
        public GroupAdapter GroupAdapter;

        public string SkinClass
        {
            get
            {
                if (null != SkinnableComponentAdapter)
                    return SkinnableComponentAdapter.SkinClass;
                if (null != SkinnableContainerAdapter)
                    return SkinnableContainerAdapter.SkinClass;
                return null;
            }
        }

        public bool HasAbsoluteLayout;

        public GroupAdapter ParentGroupAdapter;
        public bool HasParent;
        public bool ParentIsStage;
        
        public bool ParentUsesLayoutDescriptor;
        //public LayoutDescriptor ParentLayoutDescriptor;
        public GroupAdapter.LayoutEnum ParentLayout;
        public bool ParentHasAbsoluteLayout;
        

        public AdapterAnalysis(object target)
        {
            if (null != target)
            {
                ComponentAdapter = target as ComponentAdapter;
                SkinnableComponentAdapter = target as SkinnableComponentAdapter;
                SkinnableContainerAdapter = target as SkinnableContainerAdapter;
                GroupAdapter = target as GroupAdapter;

                if (null != GroupAdapter)
                {
                    HasAbsoluteLayout = CheckForAbsoluteLayout(GroupAdapter);
                }    
                
                if (null != ComponentAdapter && null != ComponentAdapter.transform && null != ComponentAdapter.transform.parent)
                {
                    ParentGroupAdapter = ComponentAdapter.transform.parent.GetComponent<GroupAdapter>();
                    HasParent = null != ParentGroupAdapter;
                    if (null != ParentGroupAdapter)
                    {
                        ParentIsStage = ParentGroupAdapter is StageAdapter;
                        //ParentUsesLayoutDescriptor = ParentContainerAdapter.UseLayoutDescriptor;
                        //ParentLayoutDescriptor = ParentContainerAdapter.LayoutDescriptor;
                        ParentLayout = ParentGroupAdapter.Layout;

                        ParentHasAbsoluteLayout = CheckForAbsoluteLayout(ParentGroupAdapter);
                    }
                }
            }
        }

        public bool CheckForAbsoluteLayout(GroupAdapter groupAdapter)
        {
            //return containerAdapter.UseLayoutDescriptor && containerAdapter.LayoutDescriptor == LayoutDescriptor.Absolute ||
            //                !containerAdapter.UseLayoutDescriptor && containerAdapter.Layout == ContainerAdapter.LayoutEnum.Absolute;
            return groupAdapter.Layout == GroupAdapter.LayoutEnum.Absolute;
        }

        public override string ToString()
        {
            bool hasParent = null != ParentGroupAdapter;
            return string.Format(@"ComponentAdapter: {0}
ParentContainerAdapter: {1} 
ParentIsStage: {2}
ParentIsStage: {3}
ParentHasAbsoluteLayout: {4}
ParentUsesLayoutDescriptor: {5} 
ParentLayout: {6}
ParentHasAbsoluteLayout: {7}", 
                             ComponentAdapter, 
                             hasParent ? ParentGroupAdapter.ToString() : "-",
                             hasParent, 
                             hasParent ? ParentIsStage.ToString() : "-", 
                             hasParent ? ParentHasAbsoluteLayout.ToString() : "-",
                             hasParent ? ParentUsesLayoutDescriptor.ToString() : "-",
                             //hasParent ? ParentLayoutDescriptor.ToString() : "-",
                             hasParent ? ParentLayout.ToString() : "-",
                             ParentHasAbsoluteLayout);
        }
    }
}