namespace JetDevel.Collections;

sealed class AvlBinaryTreeNode<T> : IBinaryTreeNode<T>
{
    // constructors...
    public AvlBinaryTreeNode(T key)
    {
        Key = key;
        Height = 1;
    }

    public override string ToString() => Key.ToString();

    // public properties...
    public AvlBinaryTreeNode<T> Left { get; internal set; }
    public AvlBinaryTreeNode<T> Right { get; internal set; }
    public byte Height { get; set; }
    public bool HasLeft => Left != null;
    public bool HasRight => Right != null;
    public T Key { get; private set; }

    T IBinaryTreeNode<T>.Key => Key;
    IBinaryTreeNode<T> IBinaryTreeNode<T>.Left => Left;
    IBinaryTreeNode<T> IBinaryTreeNode<T>.Right => Right;
}