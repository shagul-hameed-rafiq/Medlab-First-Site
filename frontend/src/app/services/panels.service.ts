import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BackendPanel {
  panelId: number;
  panelName: string;
  panelCode: string;
  testCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class PanelsService {
  private baseUrl = `${environment.apiUrl}/panels`;

  constructor(private http: HttpClient) { }

  getPanels(): Observable<BackendPanel[]> {
    return this.http.get<BackendPanel[]>(this.baseUrl);
  }
  getTestsByPanel(panelId: number) {
    return this.http.get<any[]>(`${environment.apiUrl}/panels/${panelId}/tests`);
  }
  getAllTests() {
    return this.http.get<any[]>(`${environment.apiUrl}/tests`);
  }
  createVisit(memberId: number, payload: any) {
    return this.http.post(`${environment.apiUrl}/members/${memberId}/visits`, payload, {
      responseType: 'text',
    });
  }
  getTests(panelId: number) {
    return this.http.get<any[]>(`${this.baseUrl}/${panelId}/tests`);
  }

  getVisitReport(visitId: number) {
    return this.http.get<any>(`${environment.apiUrl}/visits/${visitId}/report`);
  }

  evaluateVisit(visitId: number) {
    // Changing from null to {} as some .NET backends require an object to avoid 400 errors
    return this.http.post<any>(`${environment.apiUrl}/visits/${visitId}/evaluate`, {});
  }



  finalizeVisit(visitId: number, body: { panelRevisedSummary: string; testRevisions: { testId: number; revisedReport: string }[] }) {
    return this.http.post(`${environment.apiUrl}/visits/${visitId}/finalize`, body);
  }

  deleteVisit(memberId: number, visitId: number) {
    // Correcting to a more likely member-scoped route for this backend
    return this.http.delete(`${environment.apiUrl}/members/${memberId}/visits/${visitId}`);
  }
}


