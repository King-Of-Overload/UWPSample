package servlet;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintWriter;
import java.net.HttpCookie;
import java.util.Date;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.sun.org.apache.bcel.internal.generic.NEW;

/**
 * Servlet implementation class MyServer
 */
@WebServlet("/MyServer")
public class MyServer extends HttpServlet {
	private static final long serialVersionUID = 1L;

    /**
     * Default constructor. 
     */
    public MyServer() {
        // TODO Auto-generated constructor stub
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// TODO Auto-generated method stub
		//response.getWriter().append("Served at: ").append(request.getContextPath());
		doPost(request, response);
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		try {
			Thread.sleep(2000);
			//返回请求的内容
			response.setCharacterEncoding("utf-8");       
			response.setContentType("text/html; charset=utf-8");
			PrintWriter out = response.getWriter();
			out.println("客户端请求的数据内容：");
			//获取post请求传递过来的内容
			InputStream inputStream = request.getInputStream();
			ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
			byte[] buffer = new byte[1024];
			int len = -1;
			while((len = inputStream.read(buffer))!= -1) {
				outputStream.write(buffer,0,len);
			}
			byte[] tempcontent = outputStream.toByteArray();
			String content = new String(tempcontent);
			outputStream.close();
			inputStream.close();
			out.println(content);
			//Get请求获得移动端传来的数据设置请求缓存时间
			String cacheable = request.getParameter("cacheable");
			if(cacheable != null && !cacheable.equals("")) {
				//设置缓存时间
				response.setDateHeader("Expires", Integer.parseInt(cacheable));
				out.print("Get请求，当前的服务器时间是：");
				out.println(new Date());
				out.println("请求缓存" + response.getHeader("Expires")+"分钟");
			}
			//获取移动端请求的cookie
			Cookie[] cookies = request.getCookies();
			if(cookies != null &&request.getCookies().length >0) {
				out.println("Cookies:");
				for(Cookie cookie :request.getCookies()) {
					out.println(cookie.getName()+":"+cookie.getValue());
				}
			}
			//接收移动端传来的参数设置cookie值
			String setCookies = request.getParameter("setCookies");
			if(setCookies != null&& !setCookies.equals("")) {
				//创建一个持续3分钟的cookie
				Date date = new Date();
				Cookie myCookie1 = new Cookie("LastVisit", date.toLocaleString());
				myCookie1.setMaxAge(Integer.parseInt(String.valueOf(date.getTime()+3*60*1000)));
				response.addCookie(myCookie1);
				//创建一个http会话的cookie
				Cookie myCookie2 = new Cookie("SID", "31d4d96e407aad42");
				myCookie2.setHttpOnly(true);
				response.addCookie(myCookie2);
			}
			//返回流数据
			String extraData = request.getParameter("extraData");
			if(extraData != null && !extraData.equals("")) {
				int streamLength = Integer.parseInt(extraData);
				for(int i=0; i<streamLength ;i++) {
					out.print("@");
				}
			}
			out.flush();
			out.close();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}

}
