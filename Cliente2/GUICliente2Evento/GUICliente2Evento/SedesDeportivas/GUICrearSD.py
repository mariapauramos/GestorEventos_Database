import tkinter as tk
from tkinter import messagebox, ttk
import requests
from datetime import datetime

API_SEDES = "http://localhost:8092/sedes/"
API_EVENTOS = "http://localhost:8091/eventos/" 

class GUICrearSedeDeportiva(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Crear Sede Deportiva")
        self.geometry("600x580")
        self.configure(bg="white")
        self.resizable(False, False)

        tk.Label(self, text="CREAR SEDE DEPORTIVA",
                 font=("Verdana", 18, "bold"), bg="white").place(x=140, y=35)

        self.entries = {}
        self.eventos_data = {}  # nombre → idEvento

        self.crear_formulario()
        self.cargar_eventos()

    def crear_formulario(self):
        y = 120

        # ID SEDE
        tk.Label(self, text="ID Sede:", font=("Verdana", 11), bg="white").place(x=50, y=y)
        self.entry_id = tk.Entry(self, font=("Verdana", 11, "bold"), fg="blue")
        self.entry_id.place(x=250, y=y, width=300)

        # CAMPOS NORMALES
        labels = ["Nombre:", "Capacidad:", "Dirección:", "Costo Mantenimiento:"]
        for i, lbl in enumerate(labels):
            tk.Label(self, text=lbl, font=("Verdana", 11), bg="white").place(x=50, y=y + (i+1)*40)
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=250, y=y + (i+1)*40, width=300)
            self.entries[lbl] = entry

        # FECHA (solo YYYY-MM-DD)
        tk.Label(self, text="Fecha Creación:", font=("Verdana", 11), bg="white").place(x=50, y=y + 5*40)
        self.entry_fecha = tk.Entry(self, font=("Verdana", 11))
        self.entry_fecha.place(x=250, y=y + 5*40, width=300)
        self.entry_fecha.insert(0, datetime.now().strftime("%Y-%m-%d"))

        # CUBIERTA (True/False)
        tk.Label(self, text="Cubierta:", font=("Verdana", 11), bg="white").place(x=50, y=y + 6*40)
        self.combo_cubierta = ttk.Combobox(self, state="readonly", font=("Verdana", 11))
        self.combo_cubierta["values"] = ["True", "False"]
        self.combo_cubierta.current(0)
        self.combo_cubierta.place(x=250, y=y + 6*40, width=300)

        # EVENTO ASOCIADO
        tk.Label(self, text="Evento Deportivo:", font=("Verdana", 11), bg="white").place(x=50, y=y + 7*40)
        self.combo_eventos = ttk.Combobox(self, state="readonly", font=("Verdana", 11))
        self.combo_eventos.place(x=250, y=y + 7*40, width=300)

        # BOTONES
        tk.Button(self, text="Crear", font=("Verdana", 11, "bold"),
                  command=self.crear_sede).place(x=180, y=530, width=100)

        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=330, y=530, width=100)

    def cargar_eventos(self):
        try:
            resp = requests.get(API_EVENTOS, auth=("admin", "admin"))
            if resp.status_code == 200:
                eventos = resp.json()

                nombres = ["-- Seleccione Evento --"]
                for ev in eventos:
                    nombres.append(ev["nombre"])
                    self.eventos_data[ev["nombre"]] = ev["idEvento"]

                self.combo_eventos["values"] = nombres
                self.combo_eventos.current(0)
            else:
                messagebox.showerror("Error", "No se pudieron cargar los eventos.")
        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al backend.\n{e}")

    def validar(self):
        try:
            int(self.entry_id.get())

            obligatorios = ["Nombre:", "Capacidad:", "Dirección:", "Costo Mantenimiento:"]
            for lbl in obligatorios:
                if not self.entries[lbl].get().strip():
                    raise ValueError("Complete todos los campos obligatorios.")

            int(self.entries["Capacidad:"].get())
            float(self.entries["Costo Mantenimiento:"].get())

            # Validar fecha LocalDate
            datetime.strptime(self.entry_fecha.get(), "%Y-%m-%d")

            # Validar evento obligado por @NotBlank
            if self.combo_eventos.get() == "-- Seleccione Evento --":
                raise ValueError("Debe seleccionar un evento.")

            return True

        except ValueError as e:
            messagebox.showwarning("Validación", str(e))
            return False

    def crear_sede(self):

        if not self.validar():
            return

        # JSON que espera tu backend
        payload = {
            "idSede": int(self.entry_id.get()),
            "nombre": self.entries["Nombre:"].get(),
            "capacidad": int(self.entries["Capacidad:"].get()),
            "direccion": self.entries["Dirección:"].get(),
            "costoMantenimiento": float(self.entries["Costo Mantenimiento:"].get()),
            "fechaCreacion": self.entry_fecha.get().strip(),  # usando localdate
            "cubierta": self.combo_cubierta.get() == "True",
            "idEventoAsociado": self.eventos_data[self.combo_eventos.get()]
        }

        print("\nJSON ENVIADO:")
        print(payload)
        print("\n")

        try:
            resp = requests.post(API_SEDES, json=payload,
                                 auth=("admin", "admin"),
                                 headers={"Content-Type": "application/json"})

            if resp.status_code in (200, 201):
                messagebox.showinfo("Éxito", "Sede creada exitosamente")
                self.limpiar()
            else:
                messagebox.showerror("Error",
                                     f"Código: {resp.status_code}\nRespuesta:\n{resp.text}")

        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al backend.\n{e}")

    def limpiar(self):
        self.entry_id.delete(0, tk.END)
        for e in self.entries.values():
            e.delete(0, tk.END)
        self.entry_fecha.delete(0, tk.END)
        self.entry_fecha.insert(0, datetime.now().strftime("%Y-%m-%d"))
        self.combo_cubierta.current(0)
        self.combo_eventos.current(0)


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUICrearSedeDeportiva(root)
    root.mainloop()