import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardOverviewDto {
  activeAssessments: number;
  totalControls: number;
  completedControls: number;
  overdueControls: number;
  averageScore: number;
  complianceLevel: string;
  upcomingDeadlines: UpcomingDeadlineDto[];
}

export interface UpcomingDeadlineDto {
  name: string;
  dueDate: string;
  daysRemaining: number;
}

export interface MyControlDto {
  id: string;
  controlName: string;
  frameworkName: string;
  status: string;
  dueDate: string;
  assignedDate: string;
}

export interface FrameworkProgressDto {
  frameworkName: string;
  totalControls: number;
  completedControls: number;
  inProgressControls: number;
  notStartedControls: number;
  compliancePercentage: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = '/api/app/dashboard';

  constructor(private http: HttpClient) {}

  getOverview(): Observable<DashboardOverviewDto> {
    return this.http.get<DashboardOverviewDto>(`${this.apiUrl}/overview`);
  }

  getMyControls(): Observable<MyControlDto[]> {
    return this.http.get<MyControlDto[]>(`${this.apiUrl}/my-controls`);
  }

  getFrameworkProgress(assessmentId?: string): Observable<FrameworkProgressDto[]> {
    const url = assessmentId 
      ? `${this.apiUrl}/framework-progress?assessmentId=${assessmentId}`
      : `${this.apiUrl}/framework-progress`;
    return this.http.get<FrameworkProgressDto[]>(url);
  }

  getPendingVerification(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending-verification`);
  }
}

