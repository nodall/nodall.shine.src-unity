using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecentList {

    List<RecentItem> recent = new List<RecentItem>();

    class RecentItem
    {
        public DateTime when;
        public string uid;
    }

    double seconds;

    public RecentList(double seconds)
    {
        this.seconds = seconds;
    }

    public bool IsInList(string id)
    {
        if (recent.Count() != 0)
        {
            recent = recent.Where(x => (DateTime.Now - x.when).TotalSeconds < seconds).ToList();
        }

        return recent.Where(x => x.uid == id).Count() > 0;
    }

    public void Add(string id)
    {
        recent.Add(new RecentItem { uid = id, when = DateTime.Now });
    }

}
