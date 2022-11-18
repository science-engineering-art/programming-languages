
linked_list(linked_list &&a) // move ctor
{
    head = a.head;
    tail = a.tail;
    size = a.size;
    a.head = a.tail = nullptr;
    a.size = 0;
}
linked_list &operator=(linked_list &&a) // move assignment ctor
{
    head = a.head;
    tail = a.tail;
    size = a.size;
    a.head = a.tail = nullptr;
    a.size = 0;

    return *this;
}

#pragma endregion
