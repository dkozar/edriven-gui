using System.Collections.Generic;
using eDriven.Gui.Components;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Selector
    /// </summary>
    public class Selector
    {
        public string Subject;
        
        internal List<CSSCondition> Conditions;

        public Selector Ancestor { get; private set;}

        internal Selector(string subject)
        {
            Subject = subject;
        }

        internal Selector(string subject, List<CSSCondition> conditions)
        {
            Subject = subject;
            Conditions = conditions;
        }

        internal Selector(string subject, List<CSSCondition> conditions, Selector ancestor)
        {
            Subject = subject;
            Conditions = conditions;
            Ancestor = ancestor;
        }

        internal int Specificity
        {
            get
            {
                int s = 0;

                if ("*" != Subject && "global" != Subject && "" != Subject)
                    s = 1;

                if (Conditions != null)
                {
                    foreach (CSSCondition condition in Conditions)
                    {
                        s += condition.Specificity;
                    }
                }

                /*if (null != MediaQueries)
                    s += MediaQueries.Count*1000;*/

                if (Ancestor != null)
                    s += Ancestor.Specificity;

                return s;
            }
        }

        //internal List<MediaQuery> MediaQueries { get; set; }

        internal bool MatchesStyleClient(IStyleClient client)
        {
            bool match = false;
            //CSSCondition condition = null;

            // If we have an ancestor then this is part of a descendant selector
            if (null != Ancestor)
            {
                if (null != Conditions)
                {
                    // First, test if the conditions match
                    foreach (CSSCondition condition in Conditions)
                    {
                        match = condition.MatchesStyleClient(client);
                        if (!match)
                            return false;
                    }
                }

                // Then reset and test if any ancestor matches
                match = false;
                IStyleClient parent = client.StyleParent;
                while (parent != null)
                {
                    if (parent.MatchesCSSType(Ancestor.Subject)
                            || "*" == Ancestor.Subject)
                    {
                        match = Ancestor.MatchesStyleClient(parent);
                        if (match)
                            break;
                    }
                    parent = parent.StyleParent;
                }
            }
            else
            {
                // Check the type selector matches
                if (Subject == "*" || Subject == "" || client.MatchesCSSType(Subject))
                {
                    match = true;
                }

                // Then check if any conditions match 
                if (match && Conditions != null)
                {
                    foreach (CSSCondition condition in Conditions)
                    {
                        match = condition.MatchesStyleClient(client);
                        if (!match)
                            return false;
                    }
                }
            }

            return match;
        }

        public override string ToString()
        {
            string s;

            //var subject = string.Format("[{0}]", Subject);
            if (Ancestor != null)
            {
                s = Ancestor + " " + Subject;
            }
            else
            {
                s = Subject;
            }

            if (Conditions != null)
            {
                foreach (CSSCondition condition in Conditions)
                {
                    s += condition.ToString();
                }
            }

            return s; 
        }

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<param name="class"></param>
        ///<param name="id"></param>
        ///<param name="pseudo"></param>
        ///<returns></returns>
        internal static Selector BuildSelector(string type, string @class, string id, string pseudo)
        {
            Selector selector = new Selector(null);
            if (!string.IsNullOrEmpty(type))
            {
                selector.Subject = type;
            }
            //Debug.Log(2);

            List<CSSCondition> conditions = new List<CSSCondition>();

            if (!string.IsNullOrEmpty(@class))
            {
                conditions.Add(new CSSCondition(CSSConditionKind.Class, @class));
            }
            //Debug.Log(3);
            if (!string.IsNullOrEmpty(id))
            {
                conditions.Add(new CSSCondition(CSSConditionKind.Id, id));
            }

            /*if (!string.IsNullOrEmpty(pseudo))
            {
                conditions.Add(new CSSCondition(CSSConditionKind.Pseudo, pseudo));
            }*/

            selector.Conditions = conditions;

            return selector;
        }

        public static Selector BuildSelector(string type, string @class, string id)
        {
            return BuildSelector(type, @class, id, null);
        }
    }
}