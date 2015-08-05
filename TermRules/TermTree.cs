using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TermTreeNamespace
{
    public class Range
    {
        public Range(Point inf)
        {
            this.inf = inf;
        }
        public Point inf { get; set; }
        public static bool operator <(Range l, Range r)
        {
            if (l == null || r == null) return false; //Нужно написать исключение!
            return (((r.inf.X > l.inf.X) && (r.inf.Y <= l.inf.Y)) || ((r.inf.X >= l.inf.X) && (r.inf.Y < l.inf.Y)));
        }
        public static bool operator >(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.inf.X < l.inf.X) && (r.inf.Y >= l.inf.Y)) || ((r.inf.X <= l.inf.X) && (r.inf.Y > l.inf.Y)));
        }
        public static bool operator <=(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.inf.X > l.inf.X) && (r.inf.Y <= l.inf.Y)) || ((r.inf.X >= l.inf.X) && (r.inf.Y < l.inf.Y))) || (l.inf.X == r.inf.X && l.inf.Y == r.inf.Y);
        }
        public static bool operator >=(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.inf.X < l.inf.X) && (r.inf.Y >= l.inf.Y)) || ((r.inf.X <= l.inf.X) && (r.inf.Y > l.inf.Y))) || (l.inf.X == r.inf.X && l.inf.Y == r.inf.Y);
        }
        public static bool Equals(Range l, Range r)
        {
            if (l == null && r == null) return true;
            if (l == null || r == null) return false;
            return (l.inf.X == r.inf.X && l.inf.Y == r.inf.Y);
        }
        public static bool NotEquals(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (l.inf.X != r.inf.X || l.inf.Y != r.inf.Y);
        }
        public bool include_range(Range r)
        {
            if (r == null) return false;
            return (((this.inf.X < r.inf.X ) && ( r.inf.Y<= this.inf.Y)) || ((this.inf.X <= r.inf.X) && (r.inf.Y < this.inf.Y)));
            //return (((this.inf.X > r.inf.X) && (this.inf.Y <= r.inf.Y)) || ((this.inf.X >= r.inf.X) && (this.inf.Y < r.inf.Y)));
        }
    }
    public class TermTree
    {
        //___________________________________________________________________________________________________________________
        // Основа класса
        //___________________________________________________________________________________________________________________

        public Range range { get; set; }
        public List<int> indexElement;
        public TermTree left { get; set; }
        public TermTree right { get; set; }
        public TermTree parent { get; set; }
        public TermTree() {
            indexElement = new List<int>();
            left = null;
            right = null;
            parent = null;
            range = null;
        }
        ~TermTree() { }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Добавление интервала
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Добавляет заданный интервал в бинарное дерево поиска
        /// </summary>
        /// <param name="new_range">Добавляемый интервал</param>
        /// <returns></returns>
        public void AddRange(Range new_range)
        {
            if (Range.Equals(range, null) || Range.Equals(range,new_range))
            {
                range = new_range;
                return;
            }
            if (new_range <= range)
            {
                if (left == null) left = new TermTree();
                AddRange(new_range, left, this);
            }
            else
            {
                if (right == null) right = new TermTree();
                AddRange(new_range, right, this);
            }
        }
        /// <summary>
        /// Вставляет интервал в определённый узел дерева
        /// </summary>
        /// <param name="new_range">Интервал</param>
        /// <param name="node">Целевой узел для вставки</param>
        /// <param name="parent">Родительский узел</param>
        private void AddRange(Range new_range, TermTree node, TermTree parent)
        {

            //if (node.range == null || node.range == new_range)
            if (Range.Equals(node.range, null) || Range.Equals(node.range, new_range))
            {
                node.range = new_range;
                node.parent = parent;
                return;
            }
            if (new_range <= node.range)
            {
                if (node.left == null) node.left = new TermTree();
                AddRange(new_range, node.left, node);
            }
            else
            {
                if (node.right == null) node.right = new TermTree();
                AddRange(new_range, node.right, node);
            }
        }
        /// <summary>
        /// Уставляет узел в определённый узел дерева
        /// </summary>
        /// <param name="new_range">Вставляемый узел</param>
        /// <param name="node">Целевой узел</param>
        /// <param name="parent">Родительский узел</param>
        private void AddRange(TermTree new_range, TermTree node, TermTree parent)
        {

            //if (node.range == null || node.range == new_range.range)
            if (Range.Equals(node.range, null) || Range.Equals(node.range, new_range))
            {
                node.range = new_range.range;
                node.left = new_range.left;
                node.right = new_range.right;
                node.parent = parent;
                return;
            }
            if (new_range.range <= node.range)
            {
                if (node.left == null) node.left = new TermTree();
                AddRange(new_range, node.left, node);
            }
            else
            {
                if (node.right == null) node.right = new TermTree();
                AddRange(new_range, node.right, node);
            }
        }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Удаление интервала
        //___________________________________________________________________________________________________________________
        public enum BinSide
        {
            Left,
            Right
        }
        /// <summary>
        /// Определяет, в какой ветви для родительского лежит данный узел
        /// </summary>
        /// <param name="delete_range"></param>
        /// <returns></returns>
        private BinSide? NodeForParent(TermTree delete_range)
        {
            if (delete_range.parent == null) return null;
            if (delete_range.parent.left == delete_range) return BinSide.Left;
            if (delete_range.parent.right == delete_range) return BinSide.Right;
            return null;
        }
        /// <summary>
        /// Удаляет узел из дерева
        /// </summary>
        /// <param name="delete_range">Удаляемый узел</param>
        public void DeleteRange(TermTree delete_range)
        {
            if (delete_range == null) return;
            var me = NodeForParent(delete_range);
            //Если у узла нет дочерних элементов, его можно смело удалять
            if (delete_range.left == null && delete_range.right == null)
            {
                if (me == BinSide.Left)
                {
                    delete_range.parent.left = null;
                }
                else if (me == BinSide.Right)
                {
                    delete_range.parent.right = null;
                }
                else if (me == null)
                {
                    delete_range = null;
                }
                return;
            }
            //Если нет левого дочернего, то правый дочерний становится на место удаляемого
            if (delete_range.left == null)
            {
                if (me == BinSide.Left)
                {
                    delete_range.parent.left = delete_range.right;
                }
                else if (me == BinSide.Right)
                {
                    delete_range.parent.right = delete_range.right;
                }

                delete_range.right.parent = delete_range.parent;
                return;
            }
            //Если нет правого дочернего, то левый дочерний становится на место удаляемого
            if (delete_range.right == null)
            {
                if (me == BinSide.Left)
                {
                    delete_range.parent.left = delete_range.left;
                }
                else if (me == BinSide.Right)
                {
                    delete_range.parent.right = delete_range.left;
                }

                delete_range.left.parent = delete_range.parent;
                return;
            }

            //Если присутствуют оба дочерних узла
            //то правый ставим на место удаляемого
            //а левый вставляем в правый

            if (me == BinSide.Left)
            {
                delete_range.parent.left = delete_range.right;
            }
            if (me == BinSide.Right)
            {
                delete_range.parent.right = delete_range.right;
            }
            if (me == null)
            {
                var bufleft = delete_range.left;
                var bufrightleft = delete_range.right.left;
                var bufrightright = delete_range.right.right;
                delete_range.range = delete_range.right.range;
                delete_range.right = bufrightright;
                delete_range.left = bufrightleft;
                AddRange(bufleft, delete_range, delete_range);
            }
            else
            {
                delete_range.right.parent = delete_range.parent;
                AddRange(delete_range.left, delete_range.right, delete_range.right);
            }
        }
        /// <summary>
        /// Удаляет значение из дерева
        /// </summary>
        /// <param name="delete_range">Удаляемое значение</param>
        public void DeleteRange(Range delete_range)
        {
            var removeNode = FindRange(delete_range);
            if (removeNode != null)
            {
                DeleteRange(removeNode);
            }
        }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Поиск интервала
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Ищет узел с заданным значением
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <returns></returns>
        public TermTree FindRange(Range find_range)
        {
            if (Range.Equals(range, find_range)) return this;

            //if (range == find_range) return this;
            if (find_range <= range)
            {
                return FindRange(find_range, left);
            }
            return FindRange(find_range, right);
        }
        /// <summary>
        /// Ищет значение в определённом узле
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <param name="node">Узел для поиска</param>
        /// <returns></returns>
        public TermTree FindRange(Range find_range, TermTree node)
        {
            if (node == null) return null;

            if (Range.Equals(node.range, find_range)) return node;
            //if (node.range == find_range) return node;
            if (find_range <= node.range)
            {
                return FindRange(find_range, node.left);
            }
            return FindRange(find_range, node.right);
        }
        /// <summary>
        /// Ищет узел с заданным значением
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <returns></returns>
        public TermTree FindRangeExtension(Range find_range)
        {
            if (range.include_range(find_range)) return this;
            if (find_range <= range)
            {
                return FindRangeExtension(find_range, left);
            }
            return FindRangeExtension(find_range, right);
        }
        /// <summary>
        /// Ищет значение в определённом узле
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <param name="node">Узел для поиска</param>
        /// <returns></returns>
        public TermTree FindRangeExtension(Range find_range, TermTree node)
        {
            if (node == null) return null;

            if (node.range.include_range(find_range)) return node;
            if (find_range <= node.range)
            {
                return FindRangeExtension(find_range, node.left);
            }
            return FindRangeExtension(find_range, node.right);
        } 
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Подсчет элементов в дереве
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Количество элементов в дереве
        /// </summary>
        /// <returns></returns>
        public long CountElements()
        {
            return CountElements(this);
        }
        /// <summary>
        /// Количество элементов в определённом узле
        /// </summary>
        /// <param name="node">Узел для подсчета</param>
        /// <returns></returns>
        private long CountElements(TermTree node)
        {
            long count = 1;
            if (node.right != null)
            {
                count += CountElements(node.right);
            }
            if (node.left != null)
            {
                count += CountElements(node.left);
            }
            return count;
        }

    }
    
}
