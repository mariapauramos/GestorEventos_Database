import tkinter as tk
from tkinter import messagebox
import requests

API_SEDES = "http://localhost:8092/sedes/"

class GUIEliminarSedeDeportiva(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Eliminar Sede Deportiva")
        self.geometry("580x600")
        self.configure(bg="white")
        self.resizable(False, False)

        tk.Label(self, text="ELIMINAR SEDE DEPORTIVA",
                 font=("Verdana", 18, "bold"), bg="white").place(x=120, y=35)

        self.crear_formulario()

    # ================================
    # FORMULARIO
    # ================================
    def crear_formulario(self):

        tk.Label(self, text="ID Sede:", font=("Verdana", 11), bg="white").place(x=50, y=100)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=180, y=100, width=220)

        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  command=self.buscar_sede).place(x=420, y=100, width=90)

        labels = [
            "Nombre:", "Capacidad:", "Dirección:", "Costo Mant.:",
            "Fecha Creación:", "Cubierta:", "Evento Asociado:"
        ]

        self.entries = {}
        y_inicial = 160

        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(
                x=50, y=y_inicial + i * 40
            )

            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=200, y=y_inicial + i * 40, width=320)
            entry.config(state="readonly")
            self.entries[text] = entry

        tk.Button(self, text="Eliminar", font=("Verdana", 11, "bold"),
                  bg="lightblue", command=self.eliminar_sede).place(x=180, y=530, width=100)

        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=310, y=530, width=100)

    # ================================
    # BUSCAR SEDE
    # ================================
    def buscar_sede(self):
        idSede = self.id_entry.get().strip()

        if not idSede:
            messagebox.showwarning("Campo vacío", "Por favor ingresa un ID válido.")
            return

        try:
            resp = requests.get(API_SEDES + idSede, auth=("admin", "admin"))

            if resp.status_code == 200:
                data = resp.json()
                self.llenar_campos(data)
            else:
                messagebox.showinfo("Sin resultados",
                                    f"No existe una sede con ID: {idSede}")
                self.limpiar_campos()
        except Exception as e:
            messagebox.showerror("Error de conexión",
                                 f"No se pudo conectar al backend.\n{e}")

    # ================================
    # LLENAR CAMPOS
    # ================================
    def llenar_campos(self, data):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)

        self.entries["Nombre:"].insert(0, data.get("nombre", ""))
        self.entries["Capacidad:"].insert(0, data.get("capacidad", ""))
        self.entries["Dirección:"].insert(0, data.get("direccion", ""))
        self.entries["Costo Mant.:"].insert(0, data.get("costoMantenimiento", ""))
        self.entries["Fecha Creación:"].insert(0, data.get("fechaCreacion", ""))
        self.entries["Cubierta:"].insert(0, "Sí" if data.get("cubierta") else "No")

        self.entries["Evento Asociado:"].insert(
            0,
            data.get("nombreEvento", "Sin evento")
        )

        for entry in self.entries.values():
            entry.config(state="readonly")

    # ================================
    # LIMPIAR CAMPOS
    # ================================
    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")

    # ================================
    # ELIMINAR SEDE
    # ================================
    def eliminar_sede(self):
        idSede = self.id_entry.get().strip()

        if not idSede:
            messagebox.showwarning("Validación", "Primero busque una sede válida.")
            return

        confirm = messagebox.askyesno(
            "Confirmar eliminación",
            f"¿Está seguro que desea eliminar la sede con ID {idSede}?"
        )

        if not confirm:
            return

        try:
            resp = requests.delete(API_SEDES + idSede, auth=("admin", "admin"))

            if resp.status_code in (200, 204):
                messagebox.showinfo("Éxito", "La sede fue eliminada correctamente.")
                self.limpiar_campos()
                self.id_entry.delete(0, tk.END)
            else:
                messagebox.showerror("Error",
                                     f"No se pudo eliminar la sede.\n{resp.text}")

        except Exception as e:
            messagebox.showerror("Error",
                                 f"No se pudo conectar al servidor.\n{e}")


# Ejecutar Independiente
if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIEliminarSedeDeportiva(root)
    root.mainloop()
