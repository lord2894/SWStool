using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TermRules
{
    public class FindFunctions
    {
        public FindFunctions() { }
        public int findINList(List<Term> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (int i=0; i<v.Count;i++)
	        {
		        if (v[i].TermWord==str)
			        return i;
	        }
	        return -1;
        }
        public int findINList(List<NonDictTerm> v, string str)
        {
            if (v == null || str == null) return -1;
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].TermWord == str)
                    return i;
            }
            return -1;
        }
        public int findINList(List<CombTerm> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (int i=0; i<v.Count;i++)
	        {
		        if (v[i].TermWord==str)
			        return i;
	        }
	        return -1;
        }
        public int findINList(List<CombComponent> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (int i = 0; i<v.Count; i++)
	        {
		        if (v[i].TermWord == str)
			        return i;
	        }
	        return -1;
        }
        public int findINList(List<SynTerm> v, pair<string, string> str)
        {
            if (v == null || str == null) return -1;
	        for (int i = 0; i<v.Count; i++)
	        {
		        if ((v[i].alternatives.first == str.first && v[i].alternatives.second == str.second) || (v[i].alternatives.first == str.second && v[i].alternatives.second == str.first))
			        return i;
	        }
	        return -1;
        }
        public int findINListPos(List<Term> v, Point p)
        {
            if (v == null || p == null) return -1;
	        int sizeV=v.Count;
	        for (int i=0; i<sizeV ;i++)
	        {
		        int sizeP=v[i].Pos.Count;
		        for (int j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].range.inf.X==p.X && v[i].Pos[j].range.inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public int findINListPos(List<NonDictTerm> v, Point p)
        {
            if (v == null || p == null) return -1;
	        int sizeV=v.Count;
	        for (int i=0; i<sizeV ;i++)
	        {
		        int sizeP=v[i].Pos.Count;
		        for (int j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].range.inf.X==p.X && v[i].Pos[j].range.inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public int findINListPos(List<CombTerm> v, Point p)
        {
            if (v == null || p == null) return -1;
	        int sizeV=v.Count;
	        for (int i=0; i<sizeV ;i++)
	        {
		        int sizeP=v[i].Pos.Count;
		        for (int j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].range.inf.X==p.X && v[i].Pos[j].range.inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public ComponentInElement findINListComponents(List<NonDictTerm> v, string str)
        {
            if (v == null || str == null) return null;
	        ComponentInElement result  = new ComponentInElement();
	        result.Component=-1;
	        result.Element=-1;
	        result.Version=-1;
	        for (int i=0; i<v.Count;i++)
	        {
		        for (int k=0; k<v[i].Components.Count; k++)
		        {
			        int s=v[i].Components[k].Count;
			        for(int j=0; j<s; j++)
			        {
				        if (v[i].Components[k][j] == str)
				        {
					        result.Element=i;
					        result.Component=j;
					        result.Version=k;
					        return result;
				        }
			        }
		        }
	        }	
	        return result;
        }
        public ComponentInElement findINListComponents(List<CombTerm> v, string str)
        {
            if (v == null || str == null) return null;
	        ComponentInElement result = new ComponentInElement();
	        result.Component=-1;
	        result.Element=-1;
	        result.Version=-1;
	        for (int i=0; i<v.Count;i++)
	        {
		        int s=v[i].Components.Count;
		        for(int j=0; j<s; j++)
		        {
			        if (v[i].Components[j].TermWord == str)
			        {
				        result.Element=i;
				        result.Component=j;
				        return result;
			        }
		        }
	        }
	        return result;
        }
        public int findEqualVariants(NonDictTerm v, List<string> var)
        {
            if (v == null || var == null) return -1;
	        for (int i=0 ; i<v.Components.Count ; i++)
	        {
		        bool f=false;
		        if (v.Components[i].Count != var.Count)
			        continue;
		        for (int j=0 ; j<v.Components[i].Count; j++)
		        {
			        f=false;
			        if(v.Components[i][j] == var[j])
				        f=true;
			        if(f == false) break;
		        }
		        if(f == true) return i;
	        }
	        return -1;
        }
        public int findINListStr(List<string> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (int i=0; i<v.Count;i++)
	        {
		        if (v[i]==str)
			        return i;
	        }
	        return -1;
        }
        public int findPOS(List<Point> pos, Point p)
        {
            if (pos == null || p == null) return -1;
	        int size=pos.Count;
	        for (int i=0; i<size; i++)
		        if (pos[i].X == p.X && pos[i].Y == p.Y)
			        return i;
	        return -1;
        }
        public string GetLargestCommonSubstring(string s1, string s2)
        {
            if (s1 == null || s2 == null) return null;
            var a = new int[s1.Length + 1, s2.Length + 1];
            int u = 0, v = 0;

            for (var i = 0; i < s1.Length; i++)
                for (var j = 0; j < s2.Length; j++)
                    if (s1[i] == s2[j])
                    {
                        a[i + 1, j + 1] = a[i, j] + 1;
                        if (a[i + 1, j + 1] > a[u, v])
                        {
                            u = i + 1;
                            v = j + 1;
                        }
                    }

            return s1.Substring(u - a[u, v], a[u, v]);
        }
        public int num_spaces(string str)
        {
            if (str == null) return -1;
	        int res = 0;
	        for (int i = 0; i < str.Length; i++)
	        {
		        if (i + 1 < str.Length && str[i] == ' ' && str[i + 1] != ' ')
			        res++;
	        }
	        return res;
        }
    }
}
