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