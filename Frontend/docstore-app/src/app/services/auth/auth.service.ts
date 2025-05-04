import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID  } from '@angular/core';
import { Router } from '@angular/router';
import {jwtDecode} from 'jwt-decode'; // Install via: npm install jwt-decode
import { UserDataModel } from '../../models/UserDataModel';

@Injectable({
  providedIn: 'root',
})

export class AuthService {

  private tokenKey = 'token';
  private usernameKey = 'username';
  private allowedBucketsKey = 'allowedBuckets';

  constructor(private router: Router,@Inject(PLATFORM_ID) private platformId: Object) {}

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }
  
  isLoggedIn(): boolean {
    if (!this.isBrowser()) return false;

    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded = jwtDecode(token) as { exp: number };
      return Date.now() < decoded.exp * 1000;
    } catch (error) {
      console.error('Invalid token:', error);
      this.logout();
      return false;
    }
  }

  getToken(): string | null {
    return this.isBrowser() ? localStorage.getItem(this.tokenKey) : null;
  }

  getBuckets(): string | null {
    return this.isBrowser() ? localStorage.getItem(this.allowedBucketsKey) : null;
  }

  getUsername(): string | null {
    return this.isBrowser() ? localStorage.getItem(this.usernameKey) : null;
  }

  setUserData(data: UserDataModel): void {
    if (this.isBrowser()) {
      localStorage.setItem(this.tokenKey, data.token);
      localStorage.setItem(this.allowedBucketsKey, data.allowedBuckets);
      localStorage.setItem(this.usernameKey, data.username);
    }
  }

  logout(): void {
    if (this.isBrowser()) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.allowedBucketsKey);
      localStorage.removeItem(this.usernameKey);
      this.router.navigate(['/login']);
    }
  }
}
