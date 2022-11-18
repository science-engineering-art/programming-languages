#pragma implementation_of_list_members

template <typename T>
void linked_list<T>::add_element(T val)
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
    }
}
template <typename T>
void linked_list<T>::pop_front()
{
    if (!size)
    {
        throw range_error("");
    }
    delete_at(0);
}
template <typename T>
void linked_list<T>::pop_element()
{
    if (!size)
    {
        throw range_error("");
    }
    delete_at(size - 1);
}
template <typename T>
void linked_list<T>::delete_at(int pos)
{
    if (pos < 0 || pos >= size)
        throw range_error("");

    if (pos == 0)
    {
        head = head->Next();
        size--;
        return;
    }
    node_T *prev = element_at(pos - 1);
    if (pos == size - 1)
    {
        prev->DeleteNext();
        tail = prev;
    }
    else
    {
        node_T *nxt = element_at(pos + 1);
        prev->SetNext(*nxt);
    }

    size--;
}
template <typename T>
node<T> *linked_list<T>::element_at(int i)
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
    return element_at(i)->value;
}

#pragma end