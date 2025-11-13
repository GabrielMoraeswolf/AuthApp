import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Usuario, LoginDTO, UsuarioCreateDTO } from '../interfaces/usuario.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5269/api/auth';
  private currentUser = signal<Usuario | null>(null);

  constructor(private http: HttpClient) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage(): void {
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      try {
        this.currentUser.set(JSON.parse(savedUser));
      } catch {
        localStorage.removeItem('currentUser');
      }
    }
  }

  login(loginDTO: LoginDTO): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginDTO).pipe(
      tap(response => {
        if (response && response.id) {
          const user: Usuario = {
            id: response.id,
            login: response.login,
            nomeCompleto: response.nomeCompleto,
            email: response.email,
            acessos: response.acessos
          };
          this.currentUser.set(user);
          localStorage.setItem('currentUser', JSON.stringify(user));
        }
      })
    );
  }

  register(usuarioDTO: UsuarioCreateDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/registrar`, usuarioDTO);
  }

  getUsuarios(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(`${this.apiUrl}/usuarios`);
  }

  logout(): void {
    this.currentUser.set(null);
    localStorage.removeItem('currentUser');
  }

  getCurrentUser(): Usuario | null {
    return this.currentUser();
  }

  isAuthenticated(): boolean {
    return this.currentUser() !== null;
  }
}