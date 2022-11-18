
#pragma constructors

linked_list(const linked_list &a) // copy ctor
{
    this->operator=(a);
}
linked_list &operator=(const linked_list &a) // copy assignment ctor
{
    size = 0;
    head = tail = nullptr;
    if (a.size == 0)
    {
        return *this;
    }
    node_T *nod = a.head;
    this->Add_Last(nod->value);
    while (nod->HasNext())
    {
        nod = nod->Next();
        this->Add_Last(nod->value);
    }
    return *this;
}
