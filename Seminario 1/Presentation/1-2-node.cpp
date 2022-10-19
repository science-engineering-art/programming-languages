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
