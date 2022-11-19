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