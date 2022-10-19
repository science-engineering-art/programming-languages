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