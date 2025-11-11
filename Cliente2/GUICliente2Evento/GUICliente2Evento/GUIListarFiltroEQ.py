import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_BASE = "http://localhost:8091/equipos"

class GUIListarFiltroEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Filtrar Equipos por Número de Jugadores")
        self.geometry("850x500")
        self.configure(bg="white")
        self.resizable(False, False)
        self.crear_interfaz()
    
    def crear_interfaz(self):
       
        tk.Label(self, text="🏆 Filtrar por Número de Jugadores",
                 font=("Verdana", 16, "bold"), bg="white").place(x=170, y=20)
        
        
        tk.Label(self, text="Número Jugadores:", font=("Verdana", 11), bg="white").place(x=150, y=90)
        self.num_entry = tk.Entry(self, font=("Verdana", 11))
        self.num_entry.place(x=310, y=90, width=100)
        
        
        tk.Button(self, text="Filtrar", font=("Verdana", 11, "bold"),
                  bg="lightblue", command=self.filtrar).place(x=290, y=130, width=120)
        
       
        columnas = ("idEquipo", "Nombre", "Ciudad Origen", "Numero Jugadores", "Puntaje", "Evento Deportivo")
        self.tabla = ttk.Treeview(self, columns=columnas, show="headings", height=15)
        self.tabla.place(x=20, y=180, width=810, height=280)
        
        
        anchos = {
            "idEquipo": 80,
            "Nombre": 130,
            "Ciudad Origen": 130,
            "Numero Jugadores": 120,
            "Puntaje": 80,
            "Evento Deportivo": 150
        }
        
        for col in columnas:
            self.tabla.heading(col, text=col)
            self.tabla.column(col, width=anchos.get(col, 130), anchor="center")
        
       
        scrollbar_y = ttk.Scrollbar(self, orient="vertical", command=self.tabla.yview)
        self.tabla.configure(yscrollcommand=scrollbar_y.set)
        scrollbar_y.place(x=830, y=180, height=280)
        
        tk.Button(self, text="Cerrar", font=("Verdana", 11),
                  command=self.destroy).place(x=290, y=470, width=120)
    
    def filtrar(self):
        numero_jugadores = self.num_entry.get().strip()
        if not numero_jugadores:
            messagebox.showwarning("Campo vacío", "Ingrese el número de jugadores.")
            return
        if not numero_jugadores.isdigit():
            messagebox.showwarning("Validación", "Debe ingresar un número entero.")
            return
        
        try:
            params = {"numeroJugadores": int(numero_jugadores)}
            resp = requests.get(f"{API_BASE}/filtro", params=params, auth=("admin", "admin"))
            
            if resp.status_code == 200:
                equipos = resp.json()
                
                # Limpiar tabla
                for row in self.tabla.get_children():
                    self.tabla.delete(row)
                
                if not equipos:
                    messagebox.showinfo("Sin resultados", "No se encontraron equipos.")
                    return
                
                
                for e in equipos:
                    
                    nombre_evento = e.get("nombreEvento") or "Sin evento"
                    
                    self.tabla.insert("", "end", values=(
                        e.get("idEquipo", ""),
                        e.get("nombre", ""),
                        e.get("ciudadOrigen", ""),
                        e.get("numeroJugadores", ""),
                        e.get("puntaje", ""),
                        nombre_evento  # Nueva columna
                    ))
            else:
                messagebox.showerror("Error", f"No se pudo filtrar. Código: {resp.status_code}")
        
        except Exception as e:
            messagebox.showerror("Error de conexión", f"No se pudo conectar al servidor.\n{e}")


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIListarFiltroEQ(root)
    root.mainloop()