using System.Text.RegularExpressions;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    internal class CSSCondition
    {
        /**
         *  
         */ 
        private readonly CSSConditionKind _kind = CSSConditionKind.Class;


        /**
         *  The kind of condition this instance represents. Options are class,
         *  id and pseudo.
         */ 
        public CSSConditionKind Kind
        {
            get
            {
                return _kind;
            }
        }

        /**
         *  
         */ 
        private string _value;

        /**
         *  The value of this condition without any CSS syntax. To get a String
         *  representation that includes CSS syntax, call the toString() method.
         */ 
        public string Value
        {
            get
            {
                return _value;
            }
        }

        /**
         *  Calculates the specificity of a conditional selector in a selector
         *  chain. The total specificity is used to determine the precedence when
         *  applying several matching style declarations. id conditions contribute
         *  100 points, pseudo and class conditions each contribute 10 points.
         *  Selectors with a higher specificity override selectors of lower
         *  specificity. If selectors have equal specificity, the declaration order
         *  determines the precedence (i.e. the last one wins).
         */ 
        public int Specificity
        {
            get
            {
                if (_kind == CSSConditionKind.Id)
                    return 100;
                if (_kind == CSSConditionKind.Class)
                    return 10;
                /*if (_kind == CSSConditionKind.Pseudo)
                    return 10;*/
                return 0;
            }
        }

        public CSSCondition(CSSConditionKind kind, string value)
        {
            _kind = kind;
            _value = value;
        }

        /**
         *  Determines whether this condition matches the given component.
         * 
         *  Param: object The component to which the condition may apply.
         *  Returns: true if component is a match, otherwise false. 
         */
        ///<summary>
        ///</summary>
        ///<param name="client"></param>
        ///<returns></returns>
        internal bool MatchesStyleClient(ISimpleStyleClient client) // TEMP internal
        {
            bool match = false;

            if (_kind == CSSConditionKind.Class)
            {
                if (/*client.StyleName != null && */client.StyleName is string)
                {
                    var sn = (string) client.StyleName;

                    /*if (client.GetType().FullName == "Assets.eDriven.Skins.ImageButtonSkin")
                        Debug.Log("MatchesStyleClient for: " + client);*/

                    // Look for a match in a potential list of styleNames 
                    string[] styleNames = Regex.Split(sn, @"/\s+/");
                    for (int i = 0; i < styleNames.Length; i++)
                    {
                        if (styleNames[i] == _value)
                        {
                            match = true;
                            break;
                        }
                    }
                }
            }
            else if (_kind == CSSConditionKind.Id)
            {
                if (client.Id == _value)
                    match = true;
            }
            /*else if (_kind == CSSConditionKind.Pseudo)
            {
                if (client.MatchesCSSState(_value))
                    match = true;
            }*/

            return match;
        }

        public override string ToString()
        {
            string s;

            if (_kind == CSSConditionKind.Class)
                s = ("." + _value);
            else if (_kind == CSSConditionKind.Id)
                s = ("#" + _value);
            /*else if (_kind == CSSConditionKind.Pseudo)
                s = (":" + _value);*/
            else
                s = ""; 

            return s;
        }
    }
}