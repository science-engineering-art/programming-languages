#include <iostream>
#include <memory>
using namespace std;

template<class T>
class node
{    
    public:
        using shared = shared_ptr<node<T>>; // alias
        shared previous;
        shared next;
        T value;

        node(T value = NULL, shared previous = nullptr, shared next = nullptr)
        {
            this->value = value;
            this->previous = previous;
            this->next = next;    
        }

        node(const node<T> &other)
        {
            this->value = other.value;
            this->previous = other.previous;
            this->next = other.next;
        }

        node(node<T> &&other)
        {
            this->value = other.value;
            this->previous = other.previous;
            this->next = other.next;

            other->value = NULL;
            other->previous = nullptr;
            other->next = nullptr;
        }

        void operator =(node<T> other)
        {
            this->value = other->value;
            this->previous = other->previous;
            this->next = other->next;
        }
};

template<class T>
class linked_list
{
    private:
        using shared = shared_ptr<node<T>>; // alias
        shared first;
        shared last;

    public:
        int count;
        
        linked_list()
        {
            this->count = 0;
            this->first = nullptr;
            this->last = nullptr;
        }
};

int main()
{
    node<int> *a = new node<int>(1);
    node<int> *b = new node<int>(2);
    node<int> *c = new node<int>(3);
    
    shared_ptr<node<int>> shared_ptr_a(a);
    shared_ptr<node<int>> shared_ptr_b(b);
    shared_ptr<node<int>> shared_ptr_c(c);

    (*a).next = shared_ptr_b;
    (*b).previous = shared_ptr_a;
    (*b).next = shared_ptr_c;
    (*c).previous = shared_ptr_b;

    for (shared_ptr<node<int>> ptr = shared_ptr_a; ptr != nullptr ; ptr = ptr->next)
    {
        cout << ptr->value <<"\n";
    }
}
