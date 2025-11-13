import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { AuthService } from '../../services/auth.service';
import { Usuario, UsuarioCreateDTO } from '../../interfaces/usuario.interface';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  usuarios: Usuario[] = [];
  showForm = false;
  novoUsuario: UsuarioCreateDTO = {
    login: '',
    nomeCompleto: '',
    email: '',
    senha: ''
  };
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarUsuarios();
  }

  carregarUsuarios(): void {
    this.authService.getUsuarios().subscribe({
      next: (usuarios) => {
        this.usuarios = usuarios;
      },
      error: (error) => {
        console.error('Erro ao carregar usuários:', error);
        this.errorMessage = 'Erro ao carregar lista de usuários';
      }
    });
  }

  mostrarFormulario(): void {
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelarCadastro(): void {
    this.showForm = false;
    this.novoUsuario = { login: '', nomeCompleto: '', email: '', senha: '' };
  }

  onSubmit(): void {
    if (!this.novoUsuario.login?.trim() || 
        !this.novoUsuario.senha?.trim() || 
        !this.novoUsuario.nomeCompleto?.trim() || 
        !this.novoUsuario.email?.trim()) {
      this.errorMessage = 'Todos os campos são obrigatórios';
      return;
    }

    if (this.novoUsuario.senha.length < 6) {
      this.errorMessage = 'A senha deve ter no mínimo 6 caracteres';
      return;
    }

    if (!this.isValidEmail(this.novoUsuario.email)) {
      this.errorMessage = 'Email em formato inválido';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.register(this.novoUsuario).subscribe({
      next: (response: any) => {
        this.loading = false;
        this.successMessage = response.message || 'Usuário cadastrado com sucesso!';
        this.showForm = false;
        this.novoUsuario = { login: '', nomeCompleto: '', email: '', senha: '' };
        this.carregarUsuarios();
        
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Erro ao cadastrar usuário';
        console.error('Register error:', error);
      }
    });
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  voltarParaLogin(): void {
    this.router.navigate(['/login']);
  }
}