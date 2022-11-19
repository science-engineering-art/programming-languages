import re
from typing import Any, List, Tuple, Iterator, Generic, TypeVar

T = TypeVar('T')
class Matrix(Iterator, Generic[T]):

    def __init__(self, rows: int, cols: int, init_value=None):
        self.amount_rows: int = rows
        self.amount_cols: int = cols
        self.matrix: List[List[T]] = [[init_value 
            for _ in range(0,cols)] 
            for _ in range(0,rows)]
    
    def __add__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Sumador de matrices.
        """
        
        result = Matrix(rows=self.amount_rows, 
            cols=self.amount_cols, init_value=None)
        
        for i in range(0, self.amount_rows):
            for j in range(0, self.amount_cols):
                result[i,j] = self[i, j] + other[i, j]

        return result

    def __mul__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Multiplicador de matrices.
        """
        
        if self.amount_cols != other.amount_rows:
            raise Exception('Invalid operation.')

        result = Matrix(rows=self.amount_rows, 
            cols=other.amount_cols, init_value=None)

        for i in range(0, self.amount_rows):
            for j in range(0, other.amount_cols):
                for h in range(0, self.amount_cols):
                    if h == 0:
                        result[i,j] = self[i, h] * other[h, j]
                    else:
                        result[i,j] += self[i, h] * other[h, j]

        return result

    # hay que heredar de object para usar funciones mágicas
    def __getitem__(self, key: Tuple[int,int]):
        """
            Indizador con una sintaxis más cómoda. 
            Ejemplo: a = matrix[i,j]
        """
        
        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')

        if len(key) != 2: 
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < len(self.amount_rows) and \
            j >= 0 and j < len(self.amount_cols):
            return self.matrix[i][j]
        else:
            raise Exception('Index out of matrix.')

    def __setitem__(self, key: Tuple[int, int], value: int):
        """
            Permite setear el valor de la matriz indexada, de manera más cómoda.
            Ejemplo: `matrix[i,j] = 4`
        """

        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')
        
        if len(key) != 2:
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < len(self.matrix) and \
            j >= 0 and j < len(self.matrix[0]):
            self.matrix[i][j] = value
        else:
            raise Exception('Index out of matrix.')

    def __getattr__(self, __name: str):

        
        matched = re.match(r"_(\d+)_(\d+)", __name)
        if matched:
            i, j = matched.groups()
            i = int(i); j = int(j)
            return self[i,j]
        
        matched = re.match(r"as_([a-z]+)", __name)
        if matched:
            type = matched.groups()[0]            
            result = Matrix(self.amount_rows, self.amount_cols)
            
            for i in range(0, self.amount_rows):
                for j in range(0, self.amount_cols):
                    result[i, j] = eval(f'{type}(self.matrix[i][j])')

            return lambda: result

    def __setattr__(self, __name: str, __value: Any):
        return super().__setattr__(__name, __value)

    def __next__(self):
        for row in self.matrix:
            for i in row:
                yield i
    
    def __iter__(self):
        return self.__next__()


a: Matrix = Matrix(2,3, init_value=0)
a[0,0] = 2
b = a.as_float()
for i in b: print(i)