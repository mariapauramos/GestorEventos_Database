import tkinter as tk
from tkinter import Menu, messagebox
from PIL import Image, ImageTk  
from Equipos.GUICrearEQ import GUICrearEQ
from Equipos.GUIListarEQ import GUIListarEQ
from Equipos.GUIListarFiltroEQ import GUIListarFiltroEQ
from Equipos.GUIBuscarEQ import GUIBuscarEQ
from Equipos.GUIActualizarEQ import GUIActualizarEQ
from GUIAcercaDe import GUIAcercaDe
from Equipos.GUIEliminarEQ import GUIEliminarEQ
from SedesDeportivas.GUICrearSD import GUICrearSedeDeportiva
from SedesDeportivas.GUIBuscarSD import GUIBuscarSedeDeportiva
from SedesDeportivas.GUIListarSD import GUIListarSedeDeportiva
from SedesDeportivas.GUIListarFiltroSD import GUIListarFiltroSedeDeportiva
from SedesDeportivas.GUIActualizarSD import GUIActualizarSedeDeportiva
from SedesDeportivas.GUIEliminarSD import GUIEliminarSedeDeportiva




class GUIPrincipal:
    #VERSION FINAL DEL PROTOTIPO....
    def __init__(self, root):
        self.root = root
        self.root.title("Equipos")
        self.root.geometry("588x381")
        self.root.config(bg="white")
        self.root.resizable(False, False)

        #Barra del menu
        menubar = Menu(self.root)

        # Menú Archivo
        archivo_menu = Menu(menubar, tearoff=0)
        archivo_menu.add_command(label="Salir", command=self.salir)
        menubar.add_cascade(label="Archivo", menu=archivo_menu)

        # Menú Evento
        evento_menu = Menu(menubar, tearoff=0)
        evento_deportivo_menu = Menu(evento_menu, tearoff=0)
        evento_deportivo_menu.add_command(label="Crear", command=self.crear)
        evento_deportivo_menu.add_command(label="Buscar", command=self.buscar)
        evento_deportivo_menu.add_command(label="Listar", command=self.listar)
        evento_deportivo_menu.add_command(label="Listar por filtro", command=self.listar_filtro)
        evento_deportivo_menu.add_command(label="Actualizar", command=self.actualizar)
        evento_deportivo_menu.add_command(label="Eliminar", command=self.eliminar)
        evento_menu.add_cascade(label="Equipos Deportivos", menu=evento_deportivo_menu)
        menubar.add_cascade(label="Equipos", menu=evento_menu)

        #Menu Sedes Deportivas
        sede_menu=Menu(menubar,tearoff=0)
        sede_deportivo_menu = Menu(sede_menu, tearoff=0)
        sede_deportivo_menu.add_command(label="Crear", command=self.crearSD)
        sede_deportivo_menu.add_command(label="Buscar", command=self.buscarSD)
        sede_deportivo_menu.add_command(label="Listar", command=self.listarSD)
        sede_deportivo_menu.add_command(label="Listar por filtro", command=self.listarFiltroSD)
        sede_deportivo_menu.add_command(label="Actualizar", command=self.actualizarSD)
        sede_deportivo_menu.add_command(label="Eliminar", command=self.eliminarSD)
        sede_menu.add_cascade(label="Sedes Deportivas", menu=sede_deportivo_menu)
        menubar.add_cascade(label="Sedes", menu=sede_menu)


        # Menú Ayuda
        ayuda_menu = Menu(menubar, tearoff=0)
        ayuda_menu.add_command(label="Acerca de", command=self.acerca_de)
        menubar.add_cascade(label="Ayuda", menu=ayuda_menu)

        self.root.config(menu=menubar)

        # Imagen de
        try:
            imagen = Image.open("images/LogoEventos.jpeg")
            imagen = imagen.resize((350, 200))
            self.photo = ImageTk.PhotoImage(imagen)
            label_imagen = tk.Label(self.root, image=self.photo, bg="white")
            label_imagen.place(x=110, y=80)
        except Exception as e:
            print("Error al cargar la imagen:", e)

    def salir(self):
        self.root.quit()

    def acerca_de(self):
        GUIAcercaDe(self.root)


    #CRUD de equipos
    def crear(self):
        GUICrearEQ(self.root)

    def buscar(self):
        GUIBuscarEQ(self.root) 

    def listar(self):
        GUIListarEQ(self.root)

    def listar_filtro(self):
        GUIListarFiltroEQ(self.root)
        
    def actualizar(self):
        GUIActualizarEQ(self.root)

    def eliminar(self):
        GUIEliminarEQ(self.root)


    #CRUD de sedes deportivas\
    def crearSD(self):
        GUICrearSedeDeportiva(self.root)

    def buscarSD(self):
        GUIBuscarSedeDeportiva(self.root)

    def listarSD(self):
        GUIListarSedeDeportiva(self.root)

    def listarFiltroSD(self):
        GUIListarFiltroSedeDeportiva(self.root)

    def actualizarSD(self):
        GUIActualizarSedeDeportiva(self.root)

    def eliminarSD(self):
        GUIEliminarSedeDeportiva(self.root)

if __name__ == "__main__":
    root = tk.Tk()
    app = GUIPrincipal(root)
    root.mainloop()
