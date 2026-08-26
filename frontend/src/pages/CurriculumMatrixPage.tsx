import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Network,
  GitBranch,
  Layers,
  CheckCircle,
  AlertCircle,
  Sparkles,
  Download,
  Share2,
  BookOpen,
  Target,
  Award,
  Hash,
  Scale,
  FileCheck2,
  Plus,
  X,
  Save,
  Edit3,
  Check,
  RotateCcw,
} from 'lucide-react';

type MappingLevel = 'I' | 'R' | 'M' | 'A' | 'RA' | 'MA' | '-';

interface MatrixCell {
  courseCode: string;
  courseName: string;
  semester: number;
  plos: Record<string, MappingLevel>;
}

export const CurriculumMatrixPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/curriculum/programs')) return 'programs';
    if (location.pathname.includes('/curriculum/versions')) return 'versions';
    if (location.pathname.includes('/curriculum/pos')) return 'pos';
    if (location.pathname.includes('/curriculum/plos')) return 'plos';
    if (location.pathname.includes('/curriculum/pis')) return 'pis';
    if (location.pathname.includes('/curriculum/weight-a')) return 'weight-a';
    if (location.pathname.includes('/curriculum/clos')) return 'clos';
    return 'matrix';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isAddCourseModalOpen, setIsAddCourseModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);
  const [isEditingMatrix, setIsEditingMatrix] = useState<boolean>(false);

  // Matrix State (Editable in real-time)
  const [matrixData, setMatrixData] = useState<MatrixCell[]>([
    { courseCode: 'IT1101', courseName: 'Nhập môn Lập trình', semester: 1, plos: { PLO1: 'I', PLO2: 'I', PLO3: 'I', PLO4: '-', PLO5: '-', PLO6: 'I' } },
    { courseCode: 'IT2102', courseName: 'Cấu trúc Dữ liệu & Giải thuật', semester: 2, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: '-', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3101', courseName: 'Cơ sở Dữ liệu & SQL', semester: 3, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: 'I', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3202', courseName: 'Phân tích & Thiết kế HTTT', semester: 4, plos: { PLO1: 'R', PLO2: 'M', PLO3: 'R', PLO4: 'R', PLO5: 'R', PLO6: 'R' } },
    { courseCode: 'IT4101', courseName: 'Lập trình .NET Nâng cao', semester: 5, plos: { PLO1: 'M', PLO2: 'M', PLO3: 'A', PLO4: 'R', PLO5: 'RA', PLO6: 'R' } },
    { courseCode: 'IT4205', courseName: 'Kiểm thử Phần mềm & QA', semester: 6, plos: { PLO1: 'M', PLO2: 'R', PLO3: 'R', PLO4: 'R', PLO5: 'MA', PLO6: 'R' } },
    { courseCode: 'IT4999', courseName: 'Khóa luận Tốt nghiệp', semester: 8, plos: { PLO1: 'A', PLO2: 'A', PLO3: 'MA', PLO4: 'MA', PLO5: 'MA', PLO6: 'MA' } },
  ]);

  const [newCourseCode, setNewCourseCode] = useState('IT4301');
  const [newCourseName, setNewCourseName] = useState('Lập trình Ứng dụng Di động Flutter');
  const [newCourseSemester, setNewCourseSemester] = useState(6);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleCellChange = (courseCode: string, plo: string, newLevel: MappingLevel) => {
    setMatrixData((prev) =>
      prev.map((row) => {
        if (row.courseCode === courseCode) {
          return {
            ...row,
            plos: {
              ...row.plos,
              [plo]: newLevel,
            },
          };
        }
        return row;
      })
    );
  };

  const handleCycleCell = (courseCode: string, plo: string) => {
    const cycleOrder: MappingLevel[] = ['-', 'I', 'R', 'M', 'A', 'RA', 'MA'];
    const currentRow = matrixData.find((r) => r.courseCode === courseCode);
    const currentVal = (currentRow?.plos[plo] || '-') as MappingLevel;
    const nextIdx = (cycleOrder.indexOf(currentVal) + 1) % cycleOrder.length;
    handleCellChange(courseCode, plo, cycleOrder[nextIdx]);
  };

  const handleSaveMatrix = () => {
    setIsEditingMatrix(false);
    setToastMessage('✓ Đã lưu thành công các thay đổi trong Ma trận liên kết CĐR!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleAddCourseToMatrix = (e: React.FormEvent) => {
    e.preventDefault();
    const newRow: MatrixCell = {
      courseCode: newCourseCode,
      courseName: newCourseName,
      semester: Number(newCourseSemester),
      plos: { PLO1: 'R', PLO2: 'R', PLO3: 'M', PLO4: '-', PLO5: '-', PLO6: '-' },
    };
    setMatrixData([...matrixData, newRow]);
    setIsAddCourseModalOpen(false);
    setToastMessage(`✓ Đã thêm học phần ${newCourseCode} vào ma trận liên kết!`);
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    setIsModalOpen(false);
    setToastMessage('✓ Đã cập nhật thành công chuẩn đầu ra / chỉ số PI!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  return (
    <div className="animate-fade-in">
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* Header Banner */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Chương Trình & Chuẩn Đầu Ra
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'matrix' && 'Ma Trận Liên Kết CĐR (I/R/M/A)'}
            {activeTab === 'programs' && 'Chương Trình Đào Tạo Trực Thuộc'}
            {activeTab === 'versions' && 'Quản Lý Các Phiên Bản CTĐT'}
            {activeTab === 'pos' && 'Mục Tiêu Đào Tạo (Program Objectives - PO)'}
            {activeTab === 'plos' && 'Chuẩn Đầu Ra Chương Trình (PLO1 – PLO9)'}
            {activeTab === 'pis' && 'Chỉ Báo Thực Hiện (Performance Indicators - PI)'}
            {activeTab === 'weight-a' && 'Trọng Số Đo Trực Tiếp A Theo CTĐT (100%)'}
            {activeTab === 'clos' && 'Chuẩn Đầu Ra Học Phần (CLO)'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          {activeTab === 'matrix' && (
            <>
              {isEditingMatrix ? (
                <button onClick={handleSaveMatrix} className="btn btn-primary" style={{ backgroundColor: 'var(--emerald-600)' }}>
                  <Save size={16} />
                  <span>Lưu Ma Trận</span>
                </button>
              ) : (
                <button onClick={() => setIsEditingMatrix(true)} className="btn btn-secondary">
                  <Edit3 size={16} />
                  <span>Chỉnh Sửa Ma Trận</span>
                </button>
              )}
              <button onClick={() => setIsAddCourseModalOpen(true)} className="btn btn-primary">
                <Plus size={16} />
                <span>+ Thêm Học Phần Vào Ma Trận</span>
              </button>
            </>
          )}

          {activeTab !== 'matrix' && (
            <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
              <Plus size={16} />
              <span>+ Tạo Mới / Cập Nhật</span>
            </button>
          )}

          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Excel</span>
          </button>
        </div>
      </div>

      {/* Scope Selector Bar */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div>
            <span className="form-label">Ngành Đào Tạo</span>
            <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
              <option>Kỹ thuật Phần mềm (7480201)</option>
              <option>Khoa học Máy tính (7480101)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Phiên Bản CTĐT</span>
            <select className="form-select" style={{ width: '240px', marginTop: '0.25rem' }}>
              <option>KTPM v2023 - Áp dụng K17 (9 PLO ABET)</option>
              <option>KTPM v2022 - Áp dụng K16 (6 PLO)</option>
              <option>KTPM v2021 - Áp dụng K15 (6 PLO)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Chế Độ Thao Tác</span>
            <div style={{ marginTop: '0.5rem' }}>
              {isEditingMatrix ? (
                <span className="badge badge-warning" style={{ fontWeight: 800 }}>
                  ● ĐANG Ở CHẾ ĐỘ CHỈNH SỬA (CLICK Ô HOẶC CHỌN ĐỂ ĐỔI MỨC)
                </span>
              ) : (
                <span className="badge badge-success">CHẾ ĐỘ XEM (VIEW MODE)</span>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* TAB 1: MA TRẬN 2D (I/R/M/A) */}
      {activeTab === 'matrix' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Đóng Góp CĐR Của Các Học Phần (CTĐT K17 - 145 Tín chỉ)</h3>
              <p className="glass-card-subtitle">
                {isEditingMatrix
                  ? '👉 Bạn đang ở chế độ chỉnh sửa: Bạn có thể chọn trực tiếp mức độ đóng góp (I, R, M, A, RA, MA) cho từng học phần.'
                  : 'Quy ước chuẩn OBE: I (Introduce) ➔ R (Reinforce) ➔ M (Master) ➔ A (Direct Assessment - Đo trực tiếp)'}
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
              <span className="badge badge-primary">I: Giới thiệu</span>
              <span className="badge badge-cyan">R: Củng cố</span>
              <span className="badge badge-success">M: Thuần thục</span>
              <span className="badge badge-danger">A: Đo trực tiếp</span>
              <span className="badge badge-danger">RA / MA: Kết hợp đo</span>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Môn</th>
                  <th>Tên Học Phần</th>
                  <th style={{ width: '90px' }}>Học Kỳ</th>
                  <th style={{ textAlign: 'center' }}>PLO1 (Kiến thức)</th>
                  <th style={{ textAlign: 'center' }}>PLO2 (Thiết kế)</th>
                  <th style={{ textAlign: 'center' }}>PLO3 (Lập trình)</th>
                  <th style={{ textAlign: 'center' }}>PLO4 (Kỹ năng)</th>
                  <th style={{ textAlign: 'center' }}>PLO5 (Kiểm thử)</th>
                  <th style={{ textAlign: 'center' }}>PLO6 (Đạo đức)</th>
                </tr>
              </thead>
              <tbody>
                {matrixData.map((row) => (
                  <tr key={row.courseCode}>
                    <td><strong className="badge badge-secondary">{row.courseCode}</strong></td>
                    <td style={{ fontWeight: 600 }}>{row.courseName}</td>
                    <td>HK {row.semester}</td>
                    {['PLO1', 'PLO2', 'PLO3', 'PLO4', 'PLO5', 'PLO6'].map((plo) => {
                      const val = (row.plos[plo] || '-') as MappingLevel;
                      let badgeClass = 'badge-secondary';
                      if (val.includes('A')) badgeClass = 'badge-danger';
                      else if (val === 'M') badgeClass = 'badge-success';
                      else if (val === 'R') badgeClass = 'badge-cyan';
                      else if (val === 'I') badgeClass = 'badge-primary';

                      return (
                        <td key={plo} style={{ textAlign: 'center' }}>
                          {isEditingMatrix ? (
                            <select
                              value={val}
                              onChange={(e) => handleCellChange(row.courseCode, plo, e.target.value as MappingLevel)}
                              className="form-select"
                              style={{
                                width: '76px',
                                padding: '0.2rem 0.4rem',
                                fontSize: '0.8rem',
                                fontWeight: 800,
                                textAlign: 'center',
                                borderColor: val.includes('A') ? 'var(--rose-500)' : val === 'M' ? 'var(--emerald-500)' : 'var(--border-medium)',
                                backgroundColor: val.includes('A') ? 'rgba(244,63,94,0.15)' : 'var(--bg-surface-elevated)',
                              }}
                            >
                              <option value="-">- (Không)</option>
                              <option value="I">I (Intro)</option>
                              <option value="R">R (Reinforce)</option>
                              <option value="M">M (Master)</option>
                              <option value="A">A (Assess)</option>
                              <option value="RA">RA (R + A)</option>
                              <option value="MA">MA (M + A)</option>
                            </select>
                          ) : (
                            <button
                              type="button"
                              onClick={() => handleCycleCell(row.courseCode, plo)}
                              title="Nhấp để đổi nhanh mức độ I/R/M/A"
                              style={{ background: 'none', border: 'none', cursor: 'pointer', padding: 0 }}
                            >
                              {val !== '-' ? (
                                <span className={`badge ${badgeClass}`} style={{ minWidth: '38px', justifyContent: 'center', fontWeight: 800 }}>
                                  {val}
                                </span>
                              ) : (
                                <span style={{ color: 'var(--text-muted)', fontSize: '0.85rem' }}>-</span>
                              )}
                            </button>
                          )}
                        </td>
                      );
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Matrix Analytics & Rule Verification Bar */}
          <div style={{ marginTop: '1.25rem', padding: '1rem 1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
            <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
              <div>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Tổng số học phần trong ma trận:</span>
                <div style={{ fontWeight: 800, color: 'var(--text-primary)' }}>{matrixData.length} Môn học</div>
              </div>
              <div>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Độ phủ đo trực tiếp (A):</span>
                <div style={{ fontWeight: 800, color: 'var(--emerald-400)' }}>6 / 6 PLO đều có học phần đo A (100%)</div>
              </div>
            </div>

            <div style={{ display: 'flex', gap: '0.75rem' }}>
              {isEditingMatrix ? (
                <button onClick={handleSaveMatrix} className="btn btn-primary">
                  <Check size={16} />
                  <span>Hoàn Tất & Lưu Ma Trận</span>
                </button>
              ) : (
                <button onClick={() => setIsEditingMatrix(true)} className="btn btn-secondary">
                  <Edit3 size={16} />
                  <span>Bật Chế Độ Sửa Ma Trận</span>
                </button>
              )}
            </div>
          </div>
        </div>
      )}

      {/* TAB 2: CHƯƠNG TRÌNH ĐÀO TẠO */}
      {activeTab === 'programs' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Chương Trình Đào Tạo Trực Thuộc Khoa CNTT</h3>
              <p className="glass-card-subtitle">Định hướng nghề nghiệp và cấu trúc tổng thể</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tạo CTĐT Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Ngành</th>
                  <th>Tên Chương Trình</th>
                  <th>Số Tín Chỉ</th>
                  <th>Chuẩn Kiểm Định</th>
                  <th>Trưởng Chương Trình</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>7480201</strong></td>
                  <td style={{ fontWeight: 700, color: 'var(--primary-400)' }}>Kỹ thuật Phần mềm</td>
                  <td>145 TC</td>
                  <td><span className="badge badge-primary">ABET CAC / AUN-QA</span></td>
                  <td>TS. Lê Hải Nam</td>
                  <td><span className="badge badge-success">ĐANG ÁP DỤNG</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: PHIÊN BẢN CTĐT */}
      {activeTab === 'versions' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Quản Lý Lịch Sử Các Phiên Bản CTĐT Ngành KTPM</h3>
              <p className="glass-card-subtitle">Hỗ trợ áp dụng độc lập cho từng Khóa mà không bị xung đột</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tạo Phiên Bản CTĐT Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Phiên Bản</th>
                  <th>Khóa Áp Dụng</th>
                  <th>Số Tín Chỉ</th>
                  <th>Khung CĐR</th>
                  <th>Ngày Ban Hành</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>KTPM_2024_v3</strong></td>
                  <td>Khóa 18 (2024 - 2028)</td>
                  <td>145 TC</td>
                  <td><span className="badge badge-primary">9 PLO (ABET CAC)</span></td>
                  <td>15/08/2024</td>
                  <td><span className="badge badge-success">ACTIVE (ĐANG DÙNG)</span></td>
                </tr>
                <tr>
                  <td><strong>KTPM_2023_v2</strong></td>
                  <td>Khóa 17 (2023 - 2027)</td>
                  <td>145 TC</td>
                  <td><span className="badge badge-primary">9 PLO (ABET CAC)</span></td>
                  <td>10/08/2023</td>
                  <td><span className="badge badge-success">ACTIVE & LOCKED</span></td>
                </tr>
                <tr>
                  <td><strong>KTPM_2021_v1</strong></td>
                  <td>Khóa 15, Khóa 16</td>
                  <td>140 TC</td>
                  <td><span className="badge badge-secondary">6 PLO (AUN-QA)</span></td>
                  <td>05/08/2021</td>
                  <td><span className="badge badge-secondary">LOCKED</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 4: MỤC TIÊU ĐÀO TẠO (PO) */}
      {activeTab === 'pos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Mục Tiêu Đào Tạo (Program Objectives - PO)</h3>
              <p className="glass-card-subtitle">Định hướng năng lực sinh viên sau 3 - 5 năm tốt nghiệp</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Mục Tiêu PO</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[
              { code: 'PO1', title: 'Năng Lực Chuyên Môn & Kỹ Thuật', desc: 'Có khả năng phân tích, thiết kế, hiện thực hóa và kiểm thử các hệ thống phần mềm phức tạp trong môi trường công nghiệp.' },
              { code: 'PO2', title: 'Khả Năng Lãnh Đạo & Làm Việc Nhóm', desc: 'Có kỹ năng giao tiếp hiệu quả, làm việc nhóm đa văn hóa, và đảm nhiệm vai trò quản lý dự án phần mềm.' },
              { code: 'PO3', title: 'Học Tập Suốt Đời & Trách Nhiệm Xã Hội', desc: 'Có tinh thần tự học, thích ứng với công nghệ mới (AI, Cloud), tuân thủ đạo đức nghề nghiệp và pháp luật.' },
            ].map((po) => (
              <div key={po.code} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.35rem' }}>
                  <strong style={{ color: 'var(--primary-400)', fontSize: '1rem' }}>{po.code}: {po.title}</strong>
                  <span className="badge badge-primary">Mục tiêu cấp ngành</span>
                </div>
                <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{po.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 5: CHUẨN ĐẦU RA (PLO1 - PLO9) */}
      {activeTab === 'plos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hệ Thống Chuẩn Đầu Ra (PLO1 – PLO4 Cấp Trường, PLO5 – PLO9 Cấp Ngành)</h3>
              <p className="glass-card-subtitle">Tuân thủ nghiêm ngặt theo Khung chuẩn đầu ra ĐH Đại Nam và chuẩn kiểm định ABET</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Chuẩn Đầu Ra PLO</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PLO</th>
                  <th>Phân Cấp</th>
                  <th>Mô Tả Năng Lực Chuẩn Đầu Ra</th>
                  <th>Mức Bloom</th>
                  <th>Liên Kết PO</th>
                  <th>Ngưỡng Đạt</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'PLO1', scope: 'CẤP TRƯỜNG', desc: 'Áp dụng kiến thức toán học, khoa học cơ bản và công nghệ thông tin vào thực tiễn', bloom: 'APPLY (Mức 3)', po: 'PO1', target: '80%' },
                  { code: 'PLO2', scope: 'CẤP TRƯỜNG', desc: 'Phân tích vấn đề kỹ thuật phức tạp và áp dụng nguyên lý công nghệ để giải quyết', bloom: 'ANALYZE (Mức 4)', po: 'PO1', target: '80%' },
                  { code: 'PLO3', scope: 'CẤP TRƯỜNG', desc: 'Thiết kế, hiện thực hóa và đánh giá giải pháp phần mềm đáp ứng yêu cầu', bloom: 'CREATE (Mức 6)', po: 'PO1', target: '80%' },
                  { code: 'PLO4', scope: 'CẤP TRƯỜNG', desc: 'Giao tiếp hiệu quả trong môi trường chuyên môn bằng lời nói và văn bản', bloom: 'APPLY (Mức 3)', po: 'PO2', target: '80%' },
                  { code: 'PLO5', scope: 'CẤP NGÀNH', desc: 'Thực hiện kiểm thử phần mềm, đảm bảo chất lượng và an toàn bảo mật hệ thống', bloom: 'EVALUATE (Mức 5)', po: 'PO1', target: '80%' },
                  { code: 'PLO6', scope: 'CẤP NGÀNH', desc: 'Thể hiện trách nhiệm nghề nghiệp và đưa ra quyết định dựa trên đạo đức, pháp luật', bloom: 'EVALUATE (Mức 5)', po: 'PO3', target: '80%' },
                ].map((plo) => (
                  <tr key={plo.code}>
                    <td><strong className="badge badge-primary">{plo.code}</strong></td>
                    <td><span className={`badge ${plo.scope === 'CẤP TRƯỜNG' ? 'badge-cyan' : 'badge-secondary'}`}>{plo.scope}</span></td>
                    <td style={{ fontWeight: 600 }}>{plo.desc}</td>
                    <td><span className="badge badge-bloom badge-cyan">{plo.bloom}</span></td>
                    <td><code>{plo.po}</code></td>
                    <td style={{ color: 'var(--emerald-400)', fontWeight: 700 }}>{plo.target}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 6: CHỈ BÁO THỰC HIỆN (PI) */}
      {activeTab === 'pis' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chỉ Báo Thực Hiện (Performance Indicators - PI)</h3>
              <p className="glass-card-subtitle">Mỗi PLO được phân rã thành các chỉ báo hành vi có thể đo lường trực tiếp</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Chỉ Báo PI</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PI</th>
                  <th>Thuộc PLO</th>
                  <th>Mô Tả Hành Vi Đo Lường Cụ Thể</th>
                  <th>Mức Bloom</th>
                  <th>Học Phần Đảm Nhận Đo (A)</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { pi: 'PI 1.1', plo: 'PLO1', desc: 'Vận dụng được kiến thức cấu trúc dữ liệu và giải thuật trong giải quyết bài toán', bloom: 'APPLY (3)', course: 'IT2102 Cấu trúc dữ liệu' },
                  { pi: 'PI 2.1', plo: 'PLO2', desc: 'Phân tích và mô hình hóa yêu cầu hệ thống phần mềm bằng UML/BPMN', bloom: 'ANALYZE (4)', course: 'IT3202 Phân tích thiết kế' },
                  { pi: 'PI 3.1', plo: 'PLO3', desc: 'Xây dựng được ứng dụng Web API hoàn chỉnh có tích hợp cơ sở dữ liệu', bloom: 'CREATE (6)', course: 'IT4101 Lập trình .NET' },
                  { pi: 'PI 5.1', plo: 'PLO5', desc: 'Thiết kế Unit Test case và tự động hóa quy trình kiểm thử CI/CD', bloom: 'EVALUATE (5)', course: 'IT4101 & IT4205' },
                ].map((item) => (
                  <tr key={item.pi}>
                    <td><strong className="badge badge-cyan">{item.pi}</strong></td>
                    <td><code>{item.plo}</code></td>
                    <td style={{ fontWeight: 600 }}>{item.desc}</td>
                    <td><span className="badge badge-bloom badge-secondary">{item.bloom}</span></td>
                    <td><span className="badge badge-danger">{item.course}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 7: TRỌNG SỐ A */}
      {activeTab === 'weight-a' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Bảng Khai Báo Trọng Số Đo Trực Tiếp A Theo CTĐT</h3>
              <p className="glass-card-subtitle">Tổng trọng số đo trực tiếp của các học phần cho mỗi PI phải đạt chuẩn 100%</p>
            </div>
            <span className="badge badge-success">Tổng kiểm tra: 100% Hợp lệ</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chỉ Số PI</th>
                  <th>Học Phần Đảm Nhận Đo (A)</th>
                  <th>Bài Đánh Giá Đo Lường</th>
                  <th>Trọng Số Trong Học Phần</th>
                  <th>Trọng Số Đóng Góp Vào PI</th>
                  <th>Kiểm Tra Tổng PI</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', fontWeight: 800, color: 'var(--primary-400)' }}>PI 3.1</td>
                  <td>IT4101: Lập trình .NET</td>
                  <td>A2: Bài Thực Hành Web API</td>
                  <td>40.0%</td>
                  <td><strong>60.0%</strong></td>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', textAlign: 'center' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td>IT4999: Khóa luận Tốt nghiệp</td>
                  <td>A3: Báo cáo & Demo Đồ án</td>
                  <td>50.0%</td>
                  <td><strong>40.0%</strong></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 8: CHUẨN ĐẦU RA HỌC PHẦN (CLO) */}
      {activeTab === 'clos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chuẩn Đầu Ra Học Phần (CLO)</h3>
              <p className="glass-card-subtitle">Ánh xạ từ CLO môn học lên chỉ số PI và PLO của chương trình</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm CLO</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã CLO</th>
                  <th>Mô Tả Chuẩn Đầu Ra Môn Học</th>
                  <th>Mức Bloom</th>
                  <th>Ánh Xạ Tới Chỉ Số PI</th>
                  <th>Bài Đánh Giá</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-cyan">CLO1</strong></td>
                  <td>Xây dựng RESTful API và xử lý dữ liệu với Entity Framework Core</td>
                  <td><span className="badge badge-bloom badge-cyan">APPLY (Mức 3)</span></td>
                  <td><strong>PI 3.1</strong></td>
                  <td>Bài Thực Hành A2</td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">CLO2</strong></td>
                  <td>Áp dụng kiến trúc Clean Architecture và Dependency Injection</td>
                  <td><span className="badge badge-bloom badge-cyan">ANALYZE (Mức 4)</span></td>
                  <td><strong>PI 2.1</strong></td>
                  <td>Đồ Án A3</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ADD COURSE TO MATRIX MODAL */}
      {isAddCourseModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thêm Học Phần Vào Ma Trận CĐR
              </h3>
              <button onClick={() => setIsAddCourseModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleAddCourseToMatrix}>
              <div className="form-group">
                <label className="form-label">Mã Học Phần</label>
                <input required type="text" value={newCourseCode} onChange={(e) => setNewCourseCode(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Tên Học Phần</label>
                <input required type="text" value={newCourseName} onChange={(e) => setNewCourseName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Học Kỳ Trong CTĐT (1 - 8)</label>
                <input required type="number" min="1" max="8" value={newCourseSemester} onChange={(e) => setNewCourseSemester(Number(e.target.value))} className="form-input" />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsAddCourseModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Plus size={16} /><span>Thêm Vào Ma Trận</span></button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* GENERAL CREATE / EDIT MODAL */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thêm Mới / Cập Nhật: {activeTab === 'pos' ? 'Mục Tiêu PO' : activeTab === 'plos' ? 'Chuẩn Đầu Ra PLO' : activeTab === 'pis' ? 'Chỉ Báo PI' : 'Chuẩn Đầu Ra CLO'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Mã Chuẩn / Chỉ Báo (Code)</label>
                <input required type="text" placeholder="Ví dụ: PLO7, PI 3.2, CLO4..." className="form-input" defaultValue="PLO7" />
              </div>

              <div className="form-group">
                <label className="form-label">Mô Tả Năng Lực</label>
                <textarea required rows={3} placeholder="Nhập nội dung mô tả..." className="form-textarea" defaultValue="Làm chủ công nghệ Cloud Native và Microservices." />
              </div>

              <div className="form-group">
                <label className="form-label">Bậc Năng Lực Bloom</label>
                <select className="form-select">
                  <option>Mức 1: Remember (Nhận biết)</option>
                  <option>Mức 2: Understand (Thông hiểu)</option>
                  <option>Mức 3: Apply (Vận dụng)</option>
                  <option>Mức 4: Analyze (Phân tích)</option>
                  <option>Mức 5: Evaluate (Đánh giá)</option>
                  <option>Mức 6: Create (Sáng tạo)</option>
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Thay Đổi</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
