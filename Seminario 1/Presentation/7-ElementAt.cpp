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