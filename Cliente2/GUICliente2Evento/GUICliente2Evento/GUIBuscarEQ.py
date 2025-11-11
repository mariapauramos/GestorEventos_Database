import tkinter as tk
from tkinter import messagebox
import requests

API_BASE = "http://localhost:8091/equipos"

class GUIBuscarEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Buscar Equipo")
        self.geometry("580x540")
        self.configure(bg="white")
        self.resizable(False, False)
        
        try:
            self.img = tk.PhotoImage(file="images/cultural.png")
            tk.Label(self, image=self.img, bg="white").place(x=50, y=10, width=80, height=80)
        except:
            pass
        
        tk.Label(self, text="BUSCAR EQUIPO",
                 font=("Verdana", 18, "bold"), bg="white").place(x=150, y=35)
        
        self.crear_formulario()
    
    def crear_formulario(self):
        
        tk.Label(self, text="ID Equipo:", font=("Verdana", 11), bg="white").place(x=50, y=100)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=180, y=100, width=220)
        self.id_entry.focus_set()
        
        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  command=self.buscar_equipo).place(x=420, y=100, width=90)
       
        
        labels = ["Nombre:", "Ciudad Origen:", "N. Jugadores:", "Puntaje:", "Evento:"]
        self.entries = {}
        y_inicial = 160
        
        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(x=50, y=y_inicial + i*40)
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=180, y=y_inicial + i*40, width=330)
            entry.config(state="readonly")
            self.entries[text] = entry
        
      
        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=250, y=500, width=100)
    
    def buscar_equipo(self):
        idEquipo = self.id_entry.get().strip()
        if not idEquipo:
            messagebox.showwarning("Campo vacío", "Por favor ingresa un ID válido.")
            return
        
        try:
            auth = ("admin", "admin")
            url = f"{API_BASE}/{idEquipo}"
            resp = requests.get(url, auth=auth, headers={"Accept": "application/json"})
            
            if resp.status_code == 200:
                data = resp.json()
                self.llenar_campos(data)
            else:
                messagebox.showinfo("Sin resultados", f"No existe un equipo con ID: {idEquipo}")
                self.limpiar_campos()
                self.id_entry.focus_set()
        
        except Exception as e:
            messagebox.showerror("Error de conexión", f"No se pudo conectar al backend.\n{e}")
    
    def llenar_campos(self, data):
        self.limpiar_campos()
        
        
        for entry in self.entries.values():
            entry.config(state="normal")
        
       
        self.entries["Nombre:"].insert(0, data.get("nombre", ""))
        self.entries["Ciudad Origen:"].insert(0, data.get("ciudadOrigen", ""))
        self.entries["N. Jugadores:"].insert(0, data.get("numeroJugadores", "")) 
        self.entries["Puntaje:"].insert(0, data.get("puntaje", ""))
        
        # Obtener evento deportivo
        self.entries["Evento:"].insert(0, str(data.get("nombreEvento") or "Sin evento"))

        for entry in self.entries.values():
            entry.config(state="readonly")
    
    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIBuscarEQ(root)
    root.mainloop()