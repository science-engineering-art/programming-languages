linked_list()  //destructor
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