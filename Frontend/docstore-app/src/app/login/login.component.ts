import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent {

  authUri: string = 'http://localhost:5231/api/Auth';

  model = {
    username: '',
    password: ''
  };

  errorMessage = '';
  isLoading = false;

  constructor(private http: HttpClient, private router: Router, private auth: AuthService) {}

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.http.post<any>(this.authUri, this.model)
      .subscribe({
        next: (response: any) => {
          this.auth.setUserData(response);
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.errorMessage = error?.status == 401 ? error?.error?.result : 'Login failed';
          this.isLoading = false;
        },
        complete: () => this.isLoading = false
      });
  }
}
