template <typename T>
class linked_list
{
    using node_T = node<T>; // alias

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
