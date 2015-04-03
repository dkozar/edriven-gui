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

using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Persistence;
using UnityEngine;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class Node
    {
        public delegate void NodeTraverseDelegate(Node node);

        private readonly Transform _transform;
        public Transform Transform
        {
            get { return _transform; }
        }

        private readonly Transform _parentTransform;
        public Transform ParentTransform
        {
            get { return _parentTransform; }
        }

        /// <summary>
        /// Used by moves processor
        /// </summary>
        public Transform OldParentTransform;

        private Node _parentNode;
        public Node ParentNode
        {
            get { return _parentNode; }
        }

        private readonly List<Node> _childNodes = new List<Node>();
        public List<Node> ChildNodes
        {
            get { return _childNodes; }
        }

        public int Depth;

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly int _transformId;
        public int TransformId
        {
            get { return _transformId; }
        }

        private readonly ComponentAdapter _adapter;
        public ComponentAdapter Adapter
        {
            get { return _adapter; }
        }

        private readonly int _adapterId;
        public int AdapterId
        {
            get { return _adapterId; }
        }

        public Node(Transform transform)
        {
            _transform = transform;
            if (null != _transform)
            {
                _parentTransform = _transform.parent;
            }

            if (null == transform)
            {
                _name = "ROOT";
            }
            else
            {
                _name = transform.name;
                _transformId = transform.GetInstanceID();

                _adapter = transform.GetComponent<ComponentAdapter>();
                if (null != _adapter)
                {
                    _adapterId = _adapter.GetInstanceID();
                }
            }

            _description = string.Format("[{0}]", GuiLookup.PathToString(transform, "->"));
        }

        public void Clear()
        {
            _childNodes.Clear();
        }

        public void Add(Node node)
        {
            _childNodes.Add(node);
            node._parentNode = this;
        }

        /// <summary>
        /// Builds the node and its children using the transform relation (recursivelly!)
        /// </summary>
        public void Build()
        {
            foreach (Transform transform in Transform)
            {
                var childNode = new Node(transform) {Depth = Depth + 1, _parentNode = this};

                //ComponentAdapter adapter = childNode.Transform.gameObject.GetComponent<ComponentAdapter>();
                //if (null != adapter)
                //{
                    _childNodes.Add(childNode);
                    childNode.Build();
                //}
            }

            //PersistOrder();
        }

        /// <summary>
        /// Traverses nodes
        /// </summary>
        public void Traverse(NodeTraverseDelegate callback)
        {
            callback(this);
            foreach (Node childNode in ChildNodes)
            {
                childNode.Traverse(callback);
            }
        }

        /*public string DescribeHierarchy(bool richText)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(Indent(Depth, null != Transform ? Transform.name : "ROOT"));
            HierarchyDescriber.DescribeChildren(ref sb, this);
            return sb.ToString();
        }*/

        public string DescribeHierarchy(bool describeGroups, bool richText = false)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(Indent(Depth, null != Transform ? Transform.name : "ROOT"));
            if (describeGroups)
                HierarchyDescriber.DescribeChildrenWithGroups(ref sb, this, richText);
            else
                HierarchyDescriber.DescribeChildren(ref sb, this, richText);

            return sb.ToString();
        }

        #region Equals

        // 2 nodes are equal if their transforms are equal

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._transform, _transform);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Node)) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return (_transform != null ? _transform.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} [{1}] {2}{3}", // Depth: {1}
                null == Adapter ? string.Empty : Adapter.GetType().Name/*Name*/, /*Depth, */
                AdapterId,
                _description,
                _childNodes.Count > 0 ? string.Format("[ChildNodes: {0}]", _childNodes.Count) : string.Empty
            );
        }

        private List<Node> _nodeList;
        
        private readonly string _description;

        public List<Node> ToList()
        {
            _nodeList = new List<Node>();
            Traverse(TraverseDelegate);
            return _nodeList;
        }

        private void TraverseDelegate(Node node)
        {
            _nodeList.Add(node);
        }

        public bool HasChangedParent(Node oldSnapshot)
        {
            //if (null == Transform)
            //    return false; // the ROOT case

            return oldSnapshot.ParentTransform != ParentTransform;
        }

        public Node FindNode(int transformId)
        {
            if (_transformId == transformId)
                return this;

            foreach (Node childNode in ChildNodes)
            {
                var node = childNode.FindNode(transformId);
                if (null != node)
                    return node;
            }

            return null;
        }

        /// <summary>
        /// A cleanup method
        /// When removing the transform from hierarchy, we need to unwatch all the child adapters
        /// </summary>
        public void RemoveFromHierarchy()
        {
            // converting the hierarchy into list and then processing each child
            List<Node> list = ToList();
            foreach (var node in list)
            {
                PersistenceManager.Instance.Unwatch(node.AdapterId);
            }
        }
    }
}