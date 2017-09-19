﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace TestFilterDemo
{
    public class ExpandableListViewAdapter : BaseExpandableListAdapter,IFilterable
    {
        private Context context;
        private List<string> listGroup;
        private Dictionary<string, List<string>> listChild;
        private GroupFilter _filter;

        public ExpandableListViewAdapter(Context context, List<string> listGroup, Dictionary<string, List<string>> listChild)
        {
            this.context = context;
            this.listGroup = listGroup;
            this.listChild = listChild;
            _filter = new GroupFilter(this);
        }
        public override int GroupCount
        {
            get
            {
                return listGroup.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public Filter Filter => _filter;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            listChild.TryGetValue(listGroup[groupPosition], out result);
            return result[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            listChild.TryGetValue(listGroup[groupPosition], out result);
            return result.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Children, null);
            }
            TextView textViewItem = convertView.FindViewById<TextView>(Resource.Id.DataValue);
            string content = (string)GetChild(groupPosition, childPosition);
            textViewItem.Text = content;
            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Groups, null);
            }
            string textGroup = (string)GetGroup(groupPosition);
            TextView textViewGroup = convertView.FindViewById<TextView>(Resource.Id.Header);
            textViewGroup.Text = textGroup;
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }


        private class GroupFilter : Filter
        {
            ExpandableListViewAdapter _adapter;
            List<string> _originalList;
            public GroupFilter(ExpandableListViewAdapter adapter)
            {
                _adapter = adapter;
                _originalList = new List<string>(adapter.listGroup);
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObject = new FilterResults();
                
                //var tmpList= _originalList.Where(g => g.ToLower().Contains(constraint.ToString().ToLower()));
                //use LinQ to generate the filter result 
                var tmpList = from o in _originalList where o.ToLower().Contains(constraint.ToString().ToLower()) select o;

                returnObject.Values = FromArray(tmpList.ToArray());
                returnObject.Count = tmpList.Count();
                return returnObject;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                _adapter.listGroup = results.Values.ToArray<string>().ToList<string>();
                _adapter.NotifyDataSetChanged();
                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}