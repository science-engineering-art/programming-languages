

template <typename T>
class linked_list
{
    using node_T = node<T>;

public:
    int size;
    linked_list() : size(0) { head = nullptr, tail = nullptr; }; // ctor

    #pragma constructors
        

        linked_list(const linked_list &a) //copy ctor
        {
            this->operator=(a);
        }
        linked_list & operator=(const linked_list &a) //copy assignment ctor
        {
            size = 0;
            head = tail = nullptr;
            if(a.size == 0) {return *this;}
            node_T* nod = a.head;
            this->Add_Last(nod->value);
            while(nod->HasNext())
            {
                nod =  nod->Next();
                this->Add_Last(nod->value);
            }
            return *this;
        }
        linked_list (linked_list && a) //move ctor
        {
            head = a.head;
            tail = a.tail;
            size = a.size;
            a.head = a.tail = nullptr;
            a.size = 0;
        }
        linked_list & operator=(linked_list && a) //move assignment ctor
        {
            head = a.head;
            tail = a.tail;
            size = a.size;
            a.head = a.tail = nullptr;
            a.size = 0;

            return *this;
        }
  


    #pragma endregion

private:
    node_T *head;
    node_T *tail;
};
