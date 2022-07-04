using System;
using System.Collections.Generic;

namespace JetDevel.Collections;

// http://habrahabr.ru/post/150732/
public class AvlBinaryTreeCollection<T>: ICollection<T>, IBinaryTree<T>
{
    // fields...
    int count;
    IComparer<T> comparer;
    AvlBinaryTreeNode<T> root;
    bool wasRemoved;
    bool wasInserted;

    // constructors...
    public AvlBinaryTreeCollection() {
        comparer = Comparer<T>.Default;
    }
    public AvlBinaryTreeCollection(IComparer<T> comparer) {
        if(comparer != null)
            this.comparer = comparer;
        else
            this.comparer = Comparer<T>.Default;
    }
    public AvlBinaryTreeCollection(IEnumerable<T> collection, IComparer<T> comparer = null) : this(comparer) {
        if(collection != null)
            foreach(var item in collection)
                Add(item);
    }

    // private methods...
    static byte GetHeight(AvlBinaryTreeNode<T> node) {
        if(node != null)
            return node.Height;
        return 0;
    }
    static int GetBalanceFactor(AvlBinaryTreeNode<T> node) {
        return GetHeight(node.Right) - GetHeight(node.Left);
    }
    static void FixHeight(AvlBinaryTreeNode<T> node) {
        var heightLeft = GetHeight(node.Left);
        var heightRight = GetHeight(node.Right);
        node.Height = heightLeft > heightRight ? heightLeft : heightRight;
        node.Height += 1;
    }
    AvlBinaryTreeNode<T> FindNode(AvlBinaryTreeNode<T> rootNode, T key) {
        if(rootNode == null)
            return null;
        var comparisonResult = comparer.Compare(key, rootNode.Key);
        if(comparisonResult == 0)
            return rootNode;
        if(comparisonResult < 0)
            return FindNode(rootNode.Left, key);
        return FindNode(rootNode.Right, key);
    }
    AvlBinaryTreeNode<T> RotateRight(AvlBinaryTreeNode<T> node) // правый поворот
    {
        var result = node.Left;
        node.Left = result.Right;
        result.Right = node;
        FixHeight(node);
        FixHeight(result);
        return result;
    }
    AvlBinaryTreeNode<T> RotateLeft(AvlBinaryTreeNode<T> node) // левый поворот
    {
        var result = node.Right;
        node.Right = result.Left;
        result.Left = node;
        FixHeight(node);
        FixHeight(result);
        return result;
    }
    AvlBinaryTreeNode<T> Balance(AvlBinaryTreeNode<T> node) {
        FixHeight(node);
        var balanceFactor = GetBalanceFactor(node);
        if(balanceFactor == 2) {
            if(GetBalanceFactor(node.Right) < 0)
                node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }
        if(balanceFactor == -2) {
            if(GetBalanceFactor(node.Left) > 0)
                node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }
        return node; // балансировка не нужна
    }
    AvlBinaryTreeNode<T> Insert(AvlBinaryTreeNode<T> root, T key) // вставка ключа k в дерево с корнем root
    {
        if(root == null) {
            wasInserted = true;
            return new AvlBinaryTreeNode<T>(key);
        }
        var comparisonResult = comparer.Compare(key, root.Key);
        if(comparisonResult < 0)
            root.Left = Insert(root.Left, key);
        else if(comparisonResult > 0)
            root.Right = Insert(root.Right, key);
        else
            return root;
        return Balance(root);
    }
    AvlBinaryTreeNode<T> FindMin(AvlBinaryTreeNode<T> node) // поиск узла с минимальным ключом в дереве node 
    {
        return node.Left != null ? FindMin(node.Left) : node;
    }
    AvlBinaryTreeNode<T> RemoveMin(AvlBinaryTreeNode<T> node) // удаление узла с минимальным ключом из дерева node
    {
        if(node.Left == null)
            return node.Right;
        node.Left = RemoveMin(node.Left);
        return Balance(node);
    }
    AvlBinaryTreeNode<T> Remove(AvlBinaryTreeNode<T> node, T key) // удаление ключа key из дерева node
    {
        if(node == null)
            return null;
        var comparisonResult = comparer.Compare(key, node.Key);
        if(comparisonResult < 0)
            node.Left = Remove(node.Left, key);
        else if(comparisonResult > 0)
            node.Right = Remove(node.Right, key);
        else //  k == p->key 
        {
            var left = node.Left;
            var right = node.Right;
            wasRemoved = true;
            //delete p;
            if(right == null)
                return left;
            var min = FindMin(right);
            min.Right = RemoveMin(right);
            min.Left = left;
            return Balance(min);
        }
        return Balance(node);
    }

    // public methods...
    public void Add(T item) {
        wasInserted = false;
        root = Insert(root, item);
        if(wasInserted)
            count++;
    }
    public void Clear() {
        root = null;
        count = 0;
    }
    public bool Contains(T item) {
        return FindNode(root, item) != null;//FindNode(item) != null;
    }
    public void CopyTo(T[] array, int arrayIndex) {
        var currentIndex = arrayIndex;
        foreach(var item in this)
            array[currentIndex++] = item;
    }
    public bool Remove(T item) {
        wasRemoved = false;
        root = Remove(root, item);
        if(wasRemoved)
            count--;
        return wasRemoved;
    }
    public IEnumerator<T> GetEnumerator() {
        if(root == null)
            yield break;
        var stack = new Stack<AvlBinaryTreeNode<T>>();
        var top = root;
        while(top != null) {
            stack.Push(top);
            top = top.Left;
        }
        while(stack.Count > 0) {
            top = stack.Pop();
            yield return top.Key;
            top = top.Right;
            while(top != null) {
                stack.Push(top);
                top = top.Left;
            }
        }
    }
    public IEnumerable<T> Reverse() {
        if(root == null)
            yield break;
        var stack = new Stack<AvlBinaryTreeNode<T>>();
        var top = root;
        while(top != null) {
            stack.Push(top);
            top = top.Right;
        }
        while(stack.Count > 0) {
            top = stack.Pop();
            yield return top.Key;
            top = top.Left;
            while(top != null) {
                stack.Push(top);
                top = top.Right;
            }
        }
    }

    // public properties...

    public int Height {
        get {
            return GetHeight(root);
        }
    }
    public int MaxHeight {
        get {
            var maxHeight = (int)Math.Round(1.44 * Math.Log(count + 2, 2) - 0.328 + 0.5);
            return maxHeight;
        }
    }
    public int Count {
        get {
            return count;
        }
    }
    public bool IsReadOnly => false;
    public IBinaryTreeNode<T> Root => root;
    public T First {
        get {
            if(count == 0)
                throw new InvalidOperationException();
            var min = root;
            while(min.HasLeft)
                min = min.Left;
            return min.Key;
        }
    }
    public T Last {
        get {
            if(count == 0)
                throw new InvalidOperationException();
            var max = root;
            while(max.HasRight)
                max = max.Right;
            return max.Key;
        }
    }

    // interfaces implementation...
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void AddRange(IEnumerable<T> collection) {
        if(collection != null)
            foreach(var item in collection)
                Add(item);
    }
}