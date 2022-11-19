import re
from typing import Any, List, Tuple, TypeVar
T = TypeVar('T')


class Matrix:
    def __init__(self, rows: int, cols: int, init_value=None):
        """
            Inicializador de la clase.
        """
        self.amount_rows: int = rows
        self.amount_cols: int = cols
        self.matrix: List[List[T]] = [[init_value
                                       for _ in range(0, cols)]
                                      for _ in range(0, rows)]

    def __add__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Sumador de matrices.
        """
        result = Matrix[T](rows=self.amount_rows, 
            cols=self.amount_cols, init_value=None)

        for i in range(0, self.amount_rows):
            for j in range(0, self.amount_cols):
                result[i, j] = self[i, j] + other[i, j]

        return result

    def __mul__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Multiplicador de matrices.
        """
        if self.amount_cols != other.amount_rows:
            raise Exception('Invalid operation.')

        result = Matrix[T](rows=self.amount_rows, 
            cols=other.amount_cols, init_value=None)

        if self.amount_cols != other.amount_rows:
            raise Exception('Invalid operation.')

        result = Matrix(rows=self.amount_rows,
                        cols=other.amount_cols, init_value=None)

        for i in range(0, self.amount_rows):
            for j in range(0, other.amount_cols):
                for h in range(0, self.amount_cols):
                    if h == 0:
                        result[i, j] = self[i, h] * other[h, j]
                    else:
                        result[i, j] += self[i, h] * other[h, j]

        return result

    def __getitem__(self, key: Tuple[int, int]):
        """
            Indizador con una sintaxis más cómoda. 

            Ejemplo: a = matrix[i,j]
        """
        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')

        if len(key) != 2:
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < self.amount_rows and \
            j >= 0 and j < self.amount_cols:
            return self.matrix[i][j]
        else:
            raise Exception('Index out of matrix.')

    def __setitem__(self, key: Tuple[int, int], value: T):
        """
            Permite setear el valor de la matriz indexada, 
            de manera más cómoda.
            
            Ejemplo: `matrix[i,j] = 4`
        """
        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')

        if len(key) != 2:
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < self.amount_rows and \
            j >= 0 and j < self.amount_cols:
            self.matrix[i][j] = value
        else:
            raise Exception('Index out of matrix.')

    def __getattr__(self, __name: str):
        """
            Controla la petición de atributos que pertencen a una 
            instancia de la clase y no se encuentran inicializados.
            
            Ejemplo: 
            - `a = matrix._0_2`
            - `b = matrix.as_float()`
        """
        matched = re.match(r"_(\d+)_(\d+)", __name)
        if matched:
            i, j = matched.groups()
            i = int(i); j = int(j)
            return self[i, j]

        matched = re.match(r"as_([a-z]+)", __name)
        if matched:
            type = matched.groups()[0]
            result = Matrix(self.amount_rows, self.amount_cols)

            for i in range(0, self.amount_rows):
                for j in range(0, self.amount_cols):
                    result[i, j] = eval(f'{type}(self.matrix[i][j])')

            return lambda: result

    def __setattr__(self, __name: str, __value: Any):
        """
            Setea en los atributos de la 
        """
        matched = re.match(r"_(\d+)_(\d+)", __name)
        if matched:
            i, j = matched.groups()
            i = int(i); j = int(j)
            self[i, j] = __value
            return
        
        return super().__setattr__(__name, __value)

    def __next__(self):
        """
            Método que se encarga de generar el próximo elemento de la 
            colección, de manera `lazy`.
        """
        for row in self.matrix:
            for i in row:
                yield i

    def __iter__(self):
        """
            Método que da una forma de iterar por los elementos de una 
            colección. Es quien permite hacer construcciones del lenguaje 
            de este estilo:

            `for item in matrix: print(item)`
        """
        return self.__next__()

    def __repr__(self) -> str:
        """
            Método para imprimir la matriz de la forma: `print(matrix)`.
        """
        result = ''
        for row in self.matrix:
            result += ''.join(str(row)) + '\n' 
        return result

    def __len__(self) -> int:
        """
            Método para saber el tamaño de una matriz. 

            Ejemplo: 
            - `len(matrix)`
        """
        return self.amount_cols * self.amount_rows

    def __eq__(self, other: 'Matrix[T]') -> bool:
        """
            Redeinición del operador `==`, para poder construir
            expresiones de la forma: 
            
            `if matrix1 == matrix2: pass`. 
        """
        for i,j in zip(self,other):
            if i != j:
                return False
        return True