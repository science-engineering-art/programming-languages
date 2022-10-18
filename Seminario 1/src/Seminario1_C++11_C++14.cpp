#include <bits/stdc++.h>
#define db(x) cout << (x) << '\n';
using namespace std;



template<typename R,typename... T>
using Function = R(*)(T...args);

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

    node<T> *Next()
    {
        if (next == nullptr)
            throw range_error("");

        return next;
    }

    node<T> *Previous()
    {
        if (previous == nullptr)
            throw range_error("");

        return previous;
    }

    bool HasNext()
    {
        return !(next == nullptr);
    }
    bool HasPrevious()
    {
        return !(previous == nullptr);
    }
    void SetNext(node<T> &son)
    {
        next = &son;
    }
    void SetPrevious(node<T> &parent)
    {
        previous = &parent;
    }
    void DeleteNext()
    {
        next = nullptr;
    }
    void DeletePrevious()
    {
        previous = nullptr;
    }
    void CreateNext(T value)
    {
        next = new node<T>(value);
        next->SetPrevious(*this);
    }
    void CreatePrevious(T value)
    {
        previous = new node<T>(value);
        previous->SetNext(this);
    }

#pragma classic constructors, move constructor and assignment costructors
    node(const node &a) = default;            // copy ctor
    node &operator=(const node &a) = default; // copy assignment ctor
    node(node &&a) = default;                 // move ctor
    node operator=(node &&a);                 // move assignment ctor

#pragma endregion

};

template <typename T>
class linked_list
{
    using node_T = node<T>;

public:
    int size;
    linked_list() : size(0) { head = nullptr, tail = nullptr; }; // ctor

    void Add_Last(T val);
    void Remove_Front();
    void Remove_Last();
    void Remove_At(int pos);
    node_T *At(int i);
    T operator[](int i);

    template<typename R>
    static linked_list<R> Map(linked_list<T> lst, Function<R,T> f)
    {
        linked_list<R> out = linked_list<R>();
        for (int i = 0; i < lst.size; i++)
        {
            out.Add_Last(f(lst[i]));
        }
        return out;
        
    }

    #pragma constructors
        
        linked_list(initializer_list<T> lst)
        {
            size = 0;
            head = tail = nullptr;
            for(auto it = lst.begin();it!=lst.end();++it)
            {
                this->Add_Last(*it);
            }
        }
        
        linked_list(vector<T> & v)
        {
            size = 0;
            head = tail = nullptr;
            for_each(v.begin(),v.end(),[&](T x){this->Add_Last(x);});
        }

        ~linked_list()  //destructor
        {
            if(size == 0){return;}
            node_T* nod = head;
            while(nod->HasNext()) 
            {
                node_T* nod2 = nod->Next();
                delete nod;
                nod = nod2;
            }
            delete nod;
        }
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

#pragma implementation_of_list_members
    
    template <typename T>
    void linked_list<T>::Add_Last(T val)
    {
        if (!size)
        {
            head = tail = new node<T>(val);
            size = 1;
            return;
        }
        tail->CreateNext(val);
        // *(tail -> next) = nod;
        tail = tail->Next();
        size++;
        if (size == 2)
        {
            head->SetNext(*tail);
            tail->SetPrevious(*head);
        }
    }
    template <typename T>
    void linked_list<T>::Remove_Front()
    {
        if (!size)
        {
            throw range_error("");
        }
        Remove_At(0);
    }
    template <typename T>
    void linked_list<T>::Remove_Last()
    {
        if (!size)
        {
            throw range_error("");
        }
        Remove_At(size - 1);
    }
    template <typename T>
    void linked_list<T>::Remove_At(int pos)
    {
        if (pos < 0 || pos >= size)
            throw range_error("");

        if (pos == 0)
        {
            head = head->Next();
            head->DeletePrevious();
            size--;
            return;
        }
        node_T *prev = At(pos - 1);
        if (pos == size - 1)
        {
            prev->DeleteNext();
            tail = prev;
        }
        else
        {
            node_T *nxt = At(pos + 1);
            prev->SetNext(*nxt);
            nxt->SetPrevious(*prev);
        }

        size--;
    }
    template <typename T>
    node<T> *linked_list<T>::At(int i)
    {
        node_T *nod;
        nod = head;
        while (i--)
        {
            if (!(nod->HasNext()))
            {
                throw range_error("");
            }
            nod = nod->Next();
        }
        return nod;
    }
    template <typename T>
    T linked_list<T>::operator[](int i)
    {
        return At(i)->value;
    }

#pragma end

int add_1(int a)
{
    return a + 1;
}

int main()
{
    ios_base::sync_with_stdio(0);
    cin.tie(0);
    cout.tie(0);

    linked_list<int> a({1,2,3,4,5});
    linked_list<int> l2 = linked_list<int>::Map<int>(a,add_1);
    for(int i = 0; i < l2.size; i ++ )
        cout << l2[i] << " ";
    db("")

    return 0;
}
