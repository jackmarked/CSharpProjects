namespace JetDevel.Http.Collections {
    public interface IBinaryTreeNode<T> {
        T Key { get; }
        IBinaryTreeNode<T> Left { get; }
        IBinaryTreeNode<T> Right { get; }
    }
}