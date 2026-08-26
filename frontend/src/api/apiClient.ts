// Global API Client for OutcomeHub Backend (/api/v1)

export interface UserContext {
  principalId: string;
  roleName: string;
  scopeOrgUnitId?: string;
  scopeProgramId?: string;
  facultyName: string;
  programName: string;
}

export const defaultUserContext: UserContext = {
  principalId: '10000000-0000-7000-8000-000000000001',
  roleName: 'ADMIN',
  scopeOrgUnitId: '00000000-0000-7000-8000-000000000002',
  scopeProgramId: '30000000-0000-7000-8000-000000000001',
  facultyName: 'Khoa Công nghệ Thông tin',
  programName: 'Kỹ thuật Phần mềm (7480201)',
};

let currentUserContext: UserContext = { ...defaultUserContext };

export function setUserContext(ctx: Partial<UserContext>) {
  currentUserContext = { ...currentUserContext, ...ctx };
  localStorage.setItem('outcomehub_user_context', JSON.stringify(currentUserContext));
}

export function getUserContext(): UserContext {
  const saved = localStorage.getItem('outcomehub_user_context');
  if (saved) {
    try {
      currentUserContext = JSON.parse(saved);
    } catch {
      // fallback
    }
  }
  return currentUserContext;
}

export async function apiFetch<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const ctx = getUserContext();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    'X-Principal-Id': ctx.principalId,
    'X-Role-Name': ctx.roleName,
    ...(options.headers as Record<string, string> || {}),
  };

  const response = await fetch(endpoint, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(`API Error ${response.status}: ${errorBody || response.statusText}`);
  }

  const json = await response.json();
  // Unwrap standard ApiResponse<T> if wrapped
  return (json.data !== undefined ? json.data : json) as T;
}
