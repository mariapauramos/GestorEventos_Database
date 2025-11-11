import tkinter as tk
from tkinter import messagebox, ttk
import requests

API_EQUIPOS = "http://localhost:8091/equipos/"
API_EVENTOS = "http://localhost:8091/eventos/" 

class GUICrearEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Crear Equipo")
        self.geometry("580x520")
        self.configure(bg="white")
        self.resizable(False, False)

        
        try:
            self.img = tk.PhotoImage(file="images/cultural.png")
            tk.Label(self, image=self.img, bg="white").place(x=50, y=10, width=80, height=80)
        except:
            pass

        
        tk.Label(self, text="CREAR EQUIPO", font=("Verdana", 18, "bold"), bg="white").place(x=150, y=35)

        
        self.entries = {}
        self.eventos_data = {}   # nombreEvento → idEvento
        self.crear_formulario()
        self.cargar_eventos()

    def crear_formulario(self):

        y_inicial = 100

       
        tk.Label(self, text="ID Equipo:", font=("Verdana", 11), bg="white").place(x=50, y=y_inicial)
        self.entry_id = tk.Entry(self, font=("Verdana", 11, "bold"), fg="blue")
        self.entry_id.place(x=200, y=y_inicial, width=330)

        
        labels = ["Nombre:", "Ciudad Origen:", "Numero Jugadores:", "Puntaje:"]
        
        for i, label in enumerate(labels):
            tk.Label(self, text=label, font=("Verdana", 11), bg="white").place(x=50, y=y_inicial + (i+1)*40)
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=200, y=y_inicial + (i+1)*40, width=330)
            self.entries[label] = entry

        # 
        tk.Label(self, text="Evento (Opcional):", font=("Verdana", 11), bg="white").place(x=50, y= y_inicial + 5*40)

        self.combo_eventos = ttk.Combobox(self, state="readonly", font=("Verdana", 11))
        self.combo_eventos.place(x=200, y=y_inicial + 5*40, width=330)

       
        tk.Button(self, text="Crear", font=("Verdana", 11, "bold"),
                  command=self.crear_equipo).place(x=150, y=470, width=100)

        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=330, y=470, width=100)

    
    def cargar_eventos(self):
        try:
            resp = requests.get(API_EVENTOS, auth=("admin", "admin"))
            if resp.status_code == 200:
                eventos = resp.json()

                nombres = ["-- Sin Evento --"]  # Opción para no seleccionar evento
                for ev in eventos:
                    nombres.append(ev["nombre"])
                    self.eventos_data[ev["nombre"]] = ev["idEvento"]

                self.combo_eventos["values"] = nombres
                self.combo_eventos.current(0)  # Seleccionar "Sin Evento" por defecto
            else:
                messagebox.showerror("Error", "No se pudieron cargar los eventos.")

        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al backend.\n{e}")

   
    def validar(self):
        try:
           
            if not self.entry_id.get().strip():
                raise ValueError("El ID del equipo es obligatorio.")
            
            
            int(self.entry_id.get())
            
            
            for lbl in ["Nombre:", "Ciudad Origen:", "Numero Jugadores:", "Puntaje:"]:
                if not self.entries[lbl].get().strip():
                    raise ValueError("Los campos Nombre, Ciudad, Jugadores y Puntaje son obligatorios.")

            
            int(self.entries["Numero Jugadores:"].get())
            float(self.entries["Puntaje:"].get())

            

            return True
        except ValueError as e:
            messagebox.showwarning("Validación", str(e))
            return False


    def crear_equipo(self):
        if not self.validar():
            return

        payload = {
            "idEquipo": int(self.entry_id.get()),
            "nombre": self.entries["Nombre:"].get(),
            "ciudadOrigen": self.entries["Ciudad Origen:"].get(),
            "numeroJugadores": int(self.entries["Numero Jugadores:"].get()),
            "puntaje": float(self.entries["Puntaje:"].get())
        }

        # Solo agregar eventoDeportivo si se seleccionó uno
        nombre_evento = self.combo_eventos.get()
        if nombre_evento and nombre_evento != "-- Sin Evento --":
            id_evento_real = self.eventos_data[nombre_evento]

            payload["eventoDeportivo"] = {"idEvento": id_evento_real}  
        else:
            payload["eventoDeportivo"] = None

        try:
            resp = requests.post(API_EQUIPOS, json=payload,
                                auth=("admin", "admin"),
                                headers={"Content-Type": "application/json"})

            if resp.status_code in (200, 201):
                messagebox.showinfo("Éxito", 
                    f"Equipo creado correctamente.\n"
                    f"ID: {payload['idEquipo']}")
                self.limpiar()
            else:
                messagebox.showerror("Error", f"Código: {resp.status_code}\n{resp.text}")

        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al backend.\n{e}")

   
    def limpiar(self):
        
        self.entry_id.delete(0, tk.END)
        
        
        for entry in self.entries.values():
            entry.delete(0, tk.END)
        self.combo_eventos.current(0)  # Volver a "Sin Evento"


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUICrearEQ(root)
    root.mainloop()