template <typename T>
class node
{
private:
    node *next{nullptr};
    node *previous{nullptr};

public:

    T value;
    node() : value(0){}
    node(T val) : value(val){}

    node<T> *Next();
    node<T> *Previous();
    bool HasNext();
    bool HasPrevious();
    void SetNext(node<T> &son);
    void SetPrevious(node<T> &parent);
    void DeleteNext();
    void DeletePrevious();
    void CreateNext(T value);
    void CreatePrevious(T value);

};

template <typename T>
class linked_list
{
    using node_T = node<T>; //alias

public:
    int size;
    linked_list() : size(0) { head = nullptr, tail = nullptr; }; // ctor

    void Add_Last(T val);
    void Remove_Front();
    void Remove_Last();
    void Remove_At(int pos);
    node_T *At(int i);
    T operator[](int i);

private:
    node_T *head;
    node_T *tail;
};
