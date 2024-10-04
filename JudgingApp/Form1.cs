using OfficeOpenXml;
using System;
using System.IO;
using System.Windows.Forms;

namespace JudgingApp
{
    public partial class Form1 : Form
    {
        private string[] teamNames;  // 팀명을 저장할 배열
        private string resultFile = "result.xlsx";  // 이전 점수가 저장된 엑셀 파일 경로
        private string teamFile = "teams.xlsx";  // 이전 점수가 저장된 엑셀 파일 경로
        private string[] evaluationItems = { "창의성", "준비성", "활동성", "팀웍", "발표력", "합계" };  // 평가 항목
        private string teamsFilePath;
        private string resultFilePath;  // 이전 점수가 저장된 엑셀 파일 경로


        public Form1()
        {
            InitializeComponent();
            LoadTeamNames();  // 폼이 로드될 때 팀명을 불러옴
            lstTeams.SelectedIndexChanged += LstTeams_SelectedIndexChanged;  // 팀 선택 시 이벤트 핸들러 추가
            label1.Text = evaluationItems[0];
            label2.Text = evaluationItems[1];
            label3.Text = evaluationItems[2];
            label4.Text = evaluationItems[3];
            label5.Text = evaluationItems[4];
            label6.Text = evaluationItems[5];
        }

        // 엑셀 파일에서 팀명 불러오기
        private void LoadTeamNames()
        {
            // 팀명을 저장한 엑셀 파일 경로
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            teamsFilePath = Path.Combine(desktopPath, teamFile);
            //string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //teamsFilePath = Path.Combine(currentDirectory, teamFile);
            //MessageBox.Show(teamsFilePath);


            // 엑셀 파일에서 팀명 읽어오기
            if (File.Exists(teamsFilePath))
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(teamsFilePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];  // 첫 번째 시트
                    int rowCount = worksheet.Dimension.Rows;  // 총 행 수

                    // 팀명 배열 초기화
                    teamNames = new string[rowCount - 1]; // 첫 행은 제목이므로 제외
                    for (int i = 2; i <= rowCount; i++)  // 첫 행을 제외하고 읽기
                    {
                        teamNames[i - 2] = worksheet.Cells[i, 1].Text;
                    }
                }

                // 팀명을 ListBox에 추가
                lstTeams.Items.AddRange(teamNames);
            }
            else
            {
                MessageBox.Show("팀명을 저장한 엑셀 파일을 찾을 수 없습니다.");
            }
        }


        // 선택된 팀의 이전 점수를 불러와서 텍스트박스에 표시
        private void LoadPreviousScores(string teamName)
        {
            // 바탕화면 경로 설정
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            resultFilePath = Path.Combine(desktopPath, resultFile);  // 바탕화면에 "result.xlsx" 파일로 저장

            //string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //resultFilePath = Path.Combine(currentDirectory, resultFile);  // 바탕화면에 "result.xlsx" 파일로 저장

            if (File.Exists(resultFilePath))
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(resultFilePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];  // 첫 번째 시트
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row += 1)  // 5개의 항목마다 점수를 저장했으므로 5행씩 이동
                    {
                        if (worksheet.Cells[row, 1].Text == teamName)
                        {
                            // 해당 팀의 점수를 불러와서 텍스트 박스에 표시
                            txtScore1.Text = worksheet.Cells[row, 2].Text;  // 창의성
                            txtScore2.Text = worksheet.Cells[row, 3].Text;  // 준비성
                            txtScore3.Text = worksheet.Cells[row, 4].Text;  // 활동성
                            txtScore4.Text = worksheet.Cells[row, 5].Text;  // 팀웍
                            txtScore5.Text = worksheet.Cells[row, 6].Text;  // 발표력
                            txtSum.Text = worksheet.Cells[row, 7].Text;  // 합계
                            return;
                        }
                    }

                    // 해당 팀의 점수를 찾지 못한 경우
                    //MessageBox.Show($"{teamName}의 이전 점수를 찾을 수 없습니다.");
                    txtScore1.Text = "0";  // 창의성
                    txtScore2.Text = "0";  // 준비성
                    txtScore3.Text = "0";  // 활동성
                    txtScore4.Text = "0";  // 팀웍
                    txtScore5.Text = "0";  // 발표력
                    txtSum.Text = "0";  // 합계
                }
            }
            else
            {
                MessageBox.Show("이전 점수 파일을 찾을 수 없습니다.");
            }
        }

         
        private void btnSave_Click(object sender, EventArgs e)
        {
            SumScore();
            // 바탕화면 경로 설정
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            resultFilePath = Path.Combine(desktopPath, resultFile);  // 바탕화면에 "result.xlsx" 파일로 저장

            //string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //resultFilePath = Path.Combine(currentDirectory, resultFile);  // 바탕화면에 "result.xlsx" 파일로 저장

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet;

                if (File.Exists(resultFilePath))
                {
                    // 기존 파일이 있는 경우 FileStream을 사용하여 열기
                    using (FileStream stream = new FileStream(resultFilePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        package.Load(stream);
                    }
                    worksheet = package.Workbook.Worksheets[0];  // 첫 번째 시트
                }
                else
                {
                    // 파일이 없는 경우 새로 생성
                    worksheet = package.Workbook.Worksheets.Add("Scores");

                    // 제목 추가
                    worksheet.Cells[1, 1].Value = "팀명";
                    worksheet.Cells[1, 2].Value = evaluationItems[0]; //"창의성";
                    worksheet.Cells[1, 3].Value = evaluationItems[1]; //"준비성";
                    worksheet.Cells[1, 4].Value = evaluationItems[2]; //"활동성";
                    worksheet.Cells[1, 5].Value = evaluationItems[3]; //"팀웍";
                    worksheet.Cells[1, 6].Value = evaluationItems[4]; //"발표력";
                    worksheet.Cells[1, 7].Value = evaluationItems[5]; // "합계";
                }

                // 팀명 가져오기
                string selectedTeam = lstTeams.SelectedItem.ToString();
                int rowCount = worksheet.Dimension.Rows;

                // 기존 팀명 찾기
                int existingRow = -1;
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Text.Equals(selectedTeam, StringComparison.OrdinalIgnoreCase))
                    {
                        existingRow = row;  // 기존 팀명이 발견된 경우 해당 행 번호 저장
                        break;
                    }
                }

                if (existingRow != -1)
                {
                    // 팀명이 존재할 경우, 해당 행에 점수 덮어쓰기
                    worksheet.Cells[existingRow, 2].Value = txtScore1.Text;  // 창의성
                    worksheet.Cells[existingRow, 3].Value = txtScore2.Text;  // 준비성
                    worksheet.Cells[existingRow, 4].Value = txtScore3.Text;  // 활동성
                    worksheet.Cells[existingRow, 5].Value = txtScore4.Text;  // 팀웍
                    worksheet.Cells[existingRow, 6].Value = txtScore5.Text;  // 발표력
                    worksheet.Cells[existingRow, 7].Value = txtSum.Text;  //  합계
                }
                else
                {
                    // 팀명이 존재하지 않는 경우, 새로운 행 추가
                    int startRow = rowCount + 1;  // 현재 데이터 다음 행에 추가
                    worksheet.Cells[startRow, 1].Value = selectedTeam;  // 팀명
                    worksheet.Cells[startRow, 2].Value = txtScore1.Text;  // 창의성
                    worksheet.Cells[startRow, 3].Value = txtScore2.Text;  // 준비성
                    worksheet.Cells[startRow, 4].Value = txtScore3.Text;  // 활동성
                    worksheet.Cells[startRow, 5].Value = txtScore4.Text;  // 팀웍
                    worksheet.Cells[startRow, 6].Value = txtScore5.Text;  // 발표력
                    worksheet.Cells[startRow, 7].Value = txtSum.Text;  // 합계
                }

                package.SaveAs(new FileInfo(resultFilePath));  // 바탕화면에 파일로 저장
                MessageBox.Show("점수가 저장되었습니다.");
            }
        }

        // 팀 선택 시 이전 점수 불러오기
        private void LstTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTeam = lstTeams.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedTeam))
            {
                LoadPreviousScores(selectedTeam);  // 선택된 팀의 이전 점수를 불러옴
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // 프로그램 종료
            Application.Exit();
        }

        //private void txtScoreChanged(object sender, EventArgs e)
        //{
        //    int iTotal = int.Parse(txtScore1.Text) + int.Parse(txtScore2.Text) + int.Parse(txtScore3.Text) + int.Parse(txtScore4.Text) + int.Parse(txtScore5.Text);
        //    txtScore1.Text = iTotal.ToString();
        //}

        private void SumScore()
        {
            int iTotal = int.Parse(txtScore1.Text) + int.Parse(txtScore2.Text) + int.Parse(txtScore3.Text) + int.Parse(txtScore4.Text) + int.Parse(txtScore5.Text);
            txtSum.Text = iTotal.ToString();
        }
    }
}
